url=$(curl "https://login.ti.com/as/authorization.oauth2?response_type=code&scope=openid%20email%20profile&client_id=DCIT_ALL_WWW-PROD&state=fLoMMYH0mbjrpyark0_imiOyOCM&redirect_uri=https%3A%2F%2Fwww.ti.com.cn%2Foidc%2Fredirect_uri%2F&nonce=1blHfkkJ1i7WC2CKNgG80HnXiMsLZaNDhn8r4TTCokI&response_mode=form_post" --compressed -s -c cookie.txt | grep "/.*.ping" -o)
loginUrl=https://login.ti.com$url
curl ''${loginUrl}'' \
    -H 'Connection: keep-alive' \
    -H 'Cache-Control: max-age=0' \
    -H 'sec-ch-ua: "Chromium";v="92", " Not A;Brand";v="99", "Google Chrome";v="92"' \
    -H 'sec-ch-ua-mobile: ?0' \
    -H 'Upgrade-Insecure-Requests: 1' \
    -H 'Origin: https://login.ti.com' \
    -H 'Content-Type: application/x-www-form-urlencoded' \
    -H 'User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36' \
    -H 'Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9' \
    -H 'Sec-Fetch-Site: same-origin' \
    -H 'Sec-Fetch-Mode: navigate' \
    -H 'Sec-Fetch-User: ?1' \
    -H 'Sec-Fetch-Dest: document' \
    -H 'Referer: https://login.ti.com/' \
    -H 'Accept-Language: zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7' \
    -b cookie.txt \
    --data-raw 'pf.username=fzaw2008%40163.com&pf.pass=Wilson1234&pf.adapterId=IDPAdapterHTMLFormCIDStandard' \
    --compressed -s \
    -c cookie.txt >logintmp
code=$(grep code logintmp | awk -F'["=]' '{printf $9}')
state=$(grep state logintmp | awk -F'["=]' '{printf $9}')
rm -f logintmp
curl 'https://www.ti.com.cn/oidc/redirect_uri/' \
    -H 'Connection: keep-alive' \
    -H 'Cache-Control: max-age=0' \
    -H 'sec-ch-ua: "Chromium";v="92", " Not A;Brand";v="99", "Google Chrome";v="92"' \
    -H 'sec-ch-ua-mobile: ?0' \
    -H 'Upgrade-Insecure-Requests: 1' \
    -H 'Origin: https://login.ti.com' \
    -H 'Content-Type: application/x-www-form-urlencoded' \
    -H 'User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36' \
    -H 'Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9' \
    -H 'Sec-Fetch-Site: same-origin' \
    -H 'Sec-Fetch-Mode: navigate' \
    -H 'Sec-Fetch-User: ?1' \
    -H 'Sec-Fetch-Dest: document' \
    -H 'Referer: https://login.ti.com/' \
    -H 'Accept-Language: zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7' \
    --data-raw 'code='${code}'&state='${state}'' \
    -b cookie.txt -c cookie.txt \
    -L \
    --compressed -s

