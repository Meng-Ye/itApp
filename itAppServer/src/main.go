package main

import (
	"crypto/md5"
	"errors"
	"fmt"
	"log"
	"net/http"
	"os"
	"os/exec"
	"runtime"
	"sync"
	"time"

	"github.com/gorilla/websocket"
)

func valid() bool {
	var cmd *exec.Cmd
	var bKey, bMd5, bLicence []byte
	var err error
	// 执行单个shell命令时, 直接运行即可
	cmd = exec.Command("/bin/sh", "-c", "echo $(dmidecode)$(more /proc/cpuinfo)$(grep MemTotal /proc/meminfo) | md5sum")
	if bMd5, err = cmd.Output(); err != nil {
		fmt.Println(err)
		os.Exit(1)
	}
	cmd = exec.Command("grep ", "zw@LAPTOP-KQV8IR10", "/root/.ssh/authorized_keys")
	if bKey, err = cmd.Output(); err != nil {
		fmt.Println(err)
		os.Exit(1)
	}

	cmd = exec.Command("cat ", "/etc/itAppServer.bLicence")
	if bLicence, err = cmd.Output(); err != nil {
		fmt.Println(err)
		os.Exit(1)
	}
	str := string(bMd5) + string(bKey) + string(bLicence)
	data := []byte(str)
	has := md5.Sum(data)
	md5str1 := fmt.Sprintf("%x", has)
	return md5str1 == "3dd359b5fd668731bb7a92a1faba4b6d"
}

const (
	// 允许等待的写入时间
	writeWait = 10 * time.Second

	// Time allowed to read the next pong message from the peer.
	pongWait = 60 * time.Second

	// Send pings to peer with this period. Must be less than pongWait.
	pingPeriod = (pongWait * 9) / 10

	// Maximum message size allowed from peer.
	maxMessageSize = 512
)

// 最大的连接ID，每次连接都加1 处理
var maxConnId int64

// 客户端读写消息
type wsMessage struct {
	// websocket.TextMessage 消息类型
	messageType int
	data        []byte
}

// ws 的所有连接
// 用于广播
var wsConnAll map[int64]*wsConnection

var upgrader = websocket.Upgrader{
	ReadBufferSize:  1024,
	WriteBufferSize: 1024,
	// 允许所有的CORS 跨域请求，正式环境可以关闭
	CheckOrigin: func(r *http.Request) bool {
		return true
	},
}

// 客户端连接
type wsConnection struct {
	wsSocket *websocket.Conn // 底层websocket
	inChan   chan *wsMessage // 读队列
	outChan  chan *wsMessage // 写队列

	mutex     sync.Mutex // 避免重复关闭管道,加锁处理
	isClosed  bool
	closeChan chan byte // 关闭通知
	id        int64
}

func wsHandler(resp http.ResponseWriter, req *http.Request) {
	// 应答客户端告知升级连接为websocket
	wsSocket, err := upgrader.Upgrade(resp, req, nil)
	if err != nil {
		log.Println("升级为websocket失败", err.Error())
		return
	}
	maxConnId++
	if maxConnId > 100 {
		log.Println("加入websocket失败，超出最大限制")
		return
	}
	// TODO 如果要控制连接数可以计算，wsConnAll长度
	// 连接数保持一定数量，超过的部分不提供服务
	wsConn := &wsConnection{
		wsSocket:  wsSocket,
		inChan:    make(chan *wsMessage, 1000),
		outChan:   make(chan *wsMessage, 1000),
		closeChan: make(chan byte),
		isClosed:  false,
		id:        maxConnId,
	}
	wsConnAll[maxConnId] = wsConn
	log.Println("当前在线人数", len(wsConnAll))

	// 处理器,发送定时信息，避免意外关闭
	go wsConn.processLoop()
	// 读协程
	go wsConn.wsReadLoop()
	// 写协程
	go wsConn.wsWriteLoop()
}

// 处理队列中的消息
func (wsConn *wsConnection) processLoop() {
	// 处理消息队列中的消息
	// 获取到消息队列中的消息，处理完成后，发送消息给客户端
	for {
		msg, err := wsConn.wsRead()
		if err != nil {
			log.Println("获取消息出现错误", err.Error())
			break
		}
		log.Println("接收到消息", string(msg.data))
		// 修改以下内容把客户端传递的消息传递给处理程序
		err = wsConn.wsWrite(msg.messageType, msg.data)
		if err != nil {
			log.Println("发送消息给客户端出现错误", err.Error())
			break
		}
	}
}

// 处理消息队列中的消息
func (wsConn *wsConnection) wsReadLoop() {
	// 设置消息的最大长度
	wsConn.wsSocket.SetReadLimit(maxMessageSize)
	wsConn.wsSocket.SetReadDeadline(time.Now().Add(pongWait))
	for {
		// 读一个message
		msgType, data, err := wsConn.wsSocket.ReadMessage()
		if err != nil {
			websocket.IsUnexpectedCloseError(err, websocket.CloseGoingAway, websocket.CloseAbnormalClosure)
			log.Println("消息读取出现错误", err.Error())
			wsConn.close()
			return
		}
		req := &wsMessage{
			msgType,
			data,
		}
		// 放入请求队列,消息入栈
		select {
		case wsConn.inChan <- req:
		case <-wsConn.closeChan:
			return
		}
	}
}

// 发送消息给客户端
func (wsConn *wsConnection) wsWriteLoop() {
	ticker := time.NewTicker(pingPeriod)
	defer func() {
		ticker.Stop()
	}()
	for {
		select {
		// 取一个应答
		case msg := <-wsConn.outChan:
			// 写给websocket
			if err := wsConn.wsSocket.WriteMessage(msg.messageType, msg.data); err != nil {
				log.Println("发送消息给客户端发生错误", err.Error())
				// 切断服务
				wsConn.close()
				return
			}
		case <-wsConn.closeChan:
			// 获取到关闭通知
			return
		case <-ticker.C:
			// 出现超时情况
			wsConn.wsSocket.SetWriteDeadline(time.Now().Add(writeWait))
			if err := wsConn.wsSocket.WriteMessage(websocket.PingMessage, nil); err != nil {
				return
			}
		}
	}
}

// 写入消息到队列中
func (wsConn *wsConnection) wsWrite(messageType int, data []byte) error {
	select {
	case wsConn.outChan <- &wsMessage{messageType, data}:
	case <-wsConn.closeChan:
		return errors.New("连接已经关闭")
	}
	return nil
}

// 读取消息队列中的消息
func (wsConn *wsConnection) wsRead() (*wsMessage, error) {
	select {
	case msg := <-wsConn.inChan:
		// 获取到消息队列中的消息
		return msg, nil
	case <-wsConn.closeChan:

	}
	return nil, errors.New("连接已经关闭")
}

// 关闭连接
func (wsConn *wsConnection) close() {
	log.Println("关闭连接被调用了")
	wsConn.wsSocket.Close()
	wsConn.mutex.Lock()
	defer wsConn.mutex.Unlock()
	if !wsConn.isClosed {
		wsConn.isClosed = true
		// 删除这个连接的变量
		delete(wsConnAll, wsConn.id)
		close(wsConn.closeChan)
	}
}

// 启动程序
func StartWebsocket(addrPort string) {
	fmt.Println("ws start at " + addrPort)
	wsConnAll = make(map[int64]*wsConnection)
	http.HandleFunc("/echo", wsHandler)
	http.ListenAndServe(addrPort, nil)
}

func IndexHandler(w http.ResponseWriter, r *http.Request) {
	mystr := fmt.Sprintf("hello world %d", maxConnId)
	fmt.Fprintln(w, mystr)
}

func StartHttp(addrPort string) {
	fmt.Println("http start at " + addrPort)
	http.HandleFunc("/", IndexHandler)
	http.ListenAndServe(addrPort, nil)
}

func main() {
	if runtime.GOOS == "linux" {
		if !valid() {
			fmt.Println("请先获得授权再使用")
			os.Exit(1)
		}
	}
	// go StartHttp(":50001")
	StartWebsocket(":50000")
}
