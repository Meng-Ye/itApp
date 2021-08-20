#!/bin/bash
[ $# -ge 1 -a "$1" != "partMatch" ] && {
    for searchTerm in "$@"
    do
        tmp=$(curl 'https://www.ti.com.cn/search/gpn?searchTerm='$searchTerm'&locale=zh-CN' --compressed -s | jq -r -c ".opns[]")
        array=($tmp)
        for((i=0;i<${#array[@]} ;i++)) ;
        do
            inventory=$(./checkStore.sh ${array[i]})
            [ -n "${inventory}" -a "${inventory}" -gt 0 ] && {
                echo ${inventory}
                exit 0
            }
        done   
    done
    exit 1
}
#模糊匹配
[ $# -eq 2 -a "$1" == "partMatch" ] && {
    searchTerm=$2
    tmp=$(curl 'https://www.ti.com.cn/search/partMatch?searchTerm='$searchTerm'&locale=zh-CN' --compressed -s | jq -r -c ".[].partNumber" )
    $0 $tmp
    exit $?
}
echo "$0 搜索产品的型号"
exit 1
