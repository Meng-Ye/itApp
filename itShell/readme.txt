#!/bin/sh
searchItem() {
    echo "$1 $2"
    curl "https://www.ti.com.cn/productmodel/'$1'/orderables?locale=zh-CN&orderable='$2'" --compressed -s
}
# 搜索 获取opn
# curl "https://www.ti.com.cn/search/gpn?searchTerm=LM7480&locale=zh-CN" --compressed -s
# 响应：{"gpn":"LM7480-Q1","description":"用于驱动背对背 NFET 的 3V 至 65V、汽车理想二极管控制器","familyId":422,"familyName":" 理想二极管/ ORing控制器","familyUrl":"https://www.ti.com.cn/zh-cn/power-management/power-switches/ideal-diodes-oring-controllers/overview.html","statusId":1,"status":"正在供货","statusDescription":"此产品已上市，且可供购买。  可提供某些产品的较新替代品。 ","newFlag":true,"hasProductImage":true,"hasDatasheet":true,"activeReplacement":[],"displayParameters":true,"opns":["LM74800QDRRRQ1","LM74801QDRRRQ1"]}
# 查询库存
# curl "https://www.ti.com.cn/storeservices/cart/opninventory?opn=LM339DR"  --compressed -s | jq ".inventory"
# 响应：{"orderable_number":"LM339DR","inventory":3562850}
# 查询库存1
curl 'https://www.ti.com.cn/product-folders/customreel/inventoryforecast' \
  -H 'Connection: keep-alive' \
  -H 'sec-ch-ua: "Chromium";v="92", " Not A;Brand";v="99", "Google Chrome";v="92"' \
  -H 'tracestate: 1565136@nr=0-1-1720594-1186209984-220943124c5d4c51----1629177314195' \
  -H 'traceparent: 00-10edeb27f61859fb31b87bf029fb7230-220943124c5d4c51-01' \
  -H 'sec-ch-ua-mobile: ?0' \
  -H 'User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36' \
  -H 'newrelic: eyJ2IjpbMCwxXSwiZCI6eyJ0eSI6IkJyb3dzZXIiLCJhYyI6IjE3MjA1OTQiLCJhcCI6IjExODYyMDk5ODQiLCJpZCI6IjIyMDk0MzEyNGM1ZDRjNTEiLCJ0ciI6IjEwZWRlYjI3ZjYxODU5ZmIzMWI4N2JmMDI5ZmI3MjMwIiwidGkiOjE2MjkxNzczMTQxOTUsInRrIjoiMTU2NTEzNiJ9fQ==' \
  -H 'content-type: application/json' \
  -H 'x-sec-clge-req-type: ajax' \
  -H 'cache-control: no-store, must-revalidate' \
  -H 'expires: 0' \
  -H 'Accept: */*' \
  -H 'Origin: https://www.ti.com.cn' \
  -H 'Sec-Fetch-Site: same-origin' \
  -H 'Sec-Fetch-Mode: cors' \
  -H 'Sec-Fetch-Dest: empty' \
  -H 'Referer: https://www.ti.com.cn/product/cn/INA1620' \
  -H 'Accept-Language: zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7' \
  -b cookie_login.txt -c cookie_login.txt \
  --data-raw '[{"orderable_material":"INA1620RTWR","quantity_requested":1000,"spq":3000,"custom_reel_enabled":true}]' \
  --compressed -s 
# 加入购物车
# https://www.ti.com.cn/recaptchaproxy/occservices/v2/ti/addtocart 
# 请求：POST
curl 'https://www.ti.com.cn/recaptchaproxy/occservices/v2/ti/addtocart' \
  -H 'Connection: keep-alive' \
  -H 'sec-ch-ua: ";Not\\A\"Brand";v="99", "Chromium";v="88"' \
  -H 'sec-ch-ua-mobile: ?0' \
  -H 'User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4298.0 Safari/537.36' \
  -H 'Content-Type: application/json' \
  -H 'X-Sec-Clge-Req-Type: ajax' \
  -H 'Cache-Control: no-store, must-revalidate' \
  -H 'gRecaptcha-Response: 03AGdBq266Gxw3ftCevCpU7N40CPj_B2PJygLEYJGM017F9uukCEOwbQVWPw6ETUZUScKagKBa0ZnGh4Z_Tm82JRyAHNw_FHYMRIpSKfF4Cp1O9ZKz_uwy62Bah_lF3eGPIKE-0yYM2eDYSJeGNFcUov8bZ_Duak88OIeY_THBefMAMDPON70NsMbj2ngI6pD34-1ml8ukcSTWfuWAstXgGrg4E_yuc64VQSj61UbKoSW-UzYdRxY6GCNWCFTHts45-YsVa9ix_OPyolbuSbJcJfyWHZW-cr2vMBoSQMeePeM0DWVptDhDMOlSsNR1Xr1kq1jy6i3ph45WbF2dT1G_YvdteoMNt5_P4AuymZ0_XmDOoP2jpwuw9qoHpz6HN0p0oMQiwvUpSOS72mxIPdhIB9IdJvn-w44YuM2mN3VldYKfuv55wUu6xYGHYXoAweDK18OiV6JIkrA4' \
  -H 'Expires: 0' \
  -H 'Accept: */*' \
  -H 'Origin: https://www.ti.com.cn' \
  -H 'Sec-Fetch-Site: same-origin' \
  -H 'Sec-Fetch-Mode: cors' \
  -H 'Sec-Fetch-Dest: empty' \
  -H 'Referer: https://www.ti.com.cn/store/ti/zh/p/product/?p=LM339DR&keyMatch=LM339DR&_ticdt=MTYyOTE2OTg5N3wwMTdiNTIxNDhhMzQwMDIxZjU2NWU4N2NhYzQ0MDMwNzEwMDE2MDY5MDA4Yzd8R0ExLjMuMjk0NjY0MTk4LjE2MjkxNjk2ODQ' \
  -H 'Accept-Language: zh-CN,zh;q=0.9' \
  -b cookie.txt \
  --data-binary '{"cartRequestList":[{"packageOption":"CTX","opnId":"LM339DR","quantity":"100","tiAddtoCartSource":"store-pdp","sparam":""}],"currency":"CNY"}' \
  --compressed -s
# 响应：{"cartId":"a4748b4f-f3fa-4182-9990-7152fefded2d","statusType":"SUCCESS","statusCode":"200","message":"1 - Items added to the cart","cartResponsetList":null}
# 查看购物车
# curl "https://www.ti.com.cn/occservices/v2/ti/viewMiniCart" -b cookie_login.txt
# 响应：{"CartId":"a4748b4f-f3fa-4182-9990-7152fefded2d","CartCount":"1","StatusType":"SUCCESS","Message":null,"StatusCode":"200","Items":null,"CartTotal":null,"ConversionRate":null}

# 微信支付界面：
# https://www.ti.com.cn/store/ti/zh/checkout/buy/multi/payment-method/citcon/addSelectedPayment
# 参数：
# curl 'https://www.ti.com.cn/store/ti/zh/checkout/buy/multi/payment-method/citcon/addSelectedPayment' \
#   -H 'Connection: keep-alive' \
#   -H 'Cache-Control: max-age=0' \
#   -H 'sec-ch-ua: ";Not\\A\"Brand";v="99", "Chromium";v="88"' \
#   -H 'sec-ch-ua-mobile: ?0' \
#   -H 'Upgrade-Insecure-Requests: 1' \
#   -H 'Origin: https://www.ti.com.cn' \
#   -H 'Content-Type: application/x-www-form-urlencoded' \
#   -H 'User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4298.0 Safari/537.36' \
#   -H 'Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9' \
#   -H 'Sec-Fetch-Site: same-origin' \
#   -H 'Sec-Fetch-Mode: navigate' \
#   -H 'Sec-Fetch-User: ?1' \
#   -H 'Sec-Fetch-Dest: document' \
#   -H 'Referer: https://www.ti.com.cn/store/ti/zh/checkout/buy/multi/payment-method/citcon/addSelectedPayment' \
#   -H 'Accept-Language: zh-CN,zh;q=0.9' \
#   -H 'Cookie: JSESSIONID=53E07A6B05B6ECD47695017B930639F8.store-prod-hybris4; ti_geo=country=CN|city=FUZHOU|continent=AS|tc_ip=220.250.13.140; ti_ua=Mozilla%2f5.0%20(Windows%20NT%2010.0%3b%20Win64%3b%20x64)%20AppleWebKit%2f537.36%20(KHTML,%20like%20Gecko)%20Chrome%2f88.0.4298.0%20Safari%2f537.36; bm_mi=E68646704E73464782F88774F0FE27F1~NXNtMrX3XKYbgOlF9HkK7Rj9ZMOJ5KGIQRR/aOLEp8bPJASOSiVpSGVoWrXabgiFIayQr/7y9mwfnT8iBF4jR5Bx+EWrdfJfrim85SkIPpXU6A/hhBksVBYawfz2jAdrczFJW4napHwSSx00GCnZQ5FMj4ExhRQ3JkP/SeziAHW3hFi3LMCwJT3fBY/MvARSSmonTftEqxJGD0iTidOrndhlKLaeRt/PVS0kR6WJNAKaBuqLEcA2r93QKSBCXtNxfizuA4PUJvv9Lmh81HiggQ==; CONSENTMGR=ts:1629169682990%7Cconsent:true; tiSessionID=017b52148a340021f565e87cac44030710016069008c7; bm_sv=D6AD3C1E2CC1C559910DD0BA5ACAF97B~v6N8o7COnqhXWalmfpkZexhk2rPRNpXO0+WkhPZRqET/wpV5B47mnn/s6TAkrfuK0zbe34t6XhUdCo85ofrlWDZI+8a1zKmJ/4K8WuEUOue7l97ZtHyDu/MB+Ce7XNWM3WCku7je04tVuz2T2m+VirNewbuCOtXhd+vbI9RosYM=; user_pref_language="zh-CN"; ak_bmsc=76E085E4F615ACE62C6FA371CEED93BE~000000000000000000000000000000~YAAQVX+GfPF+yAJ7AQAAmDoUUgyYQTBgZKJ+x9agQeP2+O7E3mcPFJLnP4kbOljU0WH047ryHpA58A58tf5egp6ny53+M6g0QG/5l7mdJrXafW/yxaXdqpdxTB+/Lnuc9CSl/UzFWp1lsKVUTzDizNIZKe3Ucmi7J0SenJHByoIOUCE+0r/x74vvA77oJeYSFbsrqLLfwm7QmmQG+Co6aE8FStM1RiwBVfpasrgsql2ivPFVAZ8Gio0hbwp4sl35rI5ehwLaVysk9qzc9aLDYUHpvHXT9RyXCn7w9+mCIsa+T0ry35rYU1dJTdjVdqbQ13Y5dyibMaUl2qujI9X6YRI/b2emRST9Q0LqOxEYwHqD6lqEoUh+LPdKWsB1e4455n1lbb4sDo/0OzStdS5cg34jaZfuZlV6un3Pn4VKBT1GzeVTrB5t90alAGYk94Wh2ll/DQ==; _ga=GA1.3.294664198.1629169684; _gid=GA1.3.1628840950.1629169684; _gcl_au=1.1.1207605723.1629169685; ELOQUA=GUID=A6C2583074F6412EAC80378952619C18; user_pref_givenNameLocalLanguage="Jack"; login-check=null; login-check=null; da_sid=8FEE33EB8E33AECFCA11AA134F046F6151|3|0|3; da_lid=BCDD00D89A73EA545F40BB990D06256AE2|0|0|0; da_intState=; coveo_visitorId=01ef075c-e5ae-4085-b025-f1ab5afcc8d4; user_pref_shipTo="CN"; user_pref_currency="CNY"; ticontent=%2Ftistore; ga_content_cookie=%2Ftistore; last-domain=www.ti.com.cn; bm_sz=3CA65CEB28064F5DFA073C55ADEA7610~YAAQVX+GfNrAzQJ7AQAAYCIiUgwWnwAOLFMy8jWaLh8OXinJP89AodX6rjNOoJT1MB57zerWiTgprV/l++2z6Gbc9DkJfSvfrM7RicD5vEcARsaixmDVNqYpHRXNg0oTbxZ0Rc6iNh7tc6eJCrDCie07TRRULQMeJGnFZAJEav3TtUjidqh7tu7T7XlnOWBaDMEVwrYlAmCsbcRZVms3C0zIpTgzPacBKhiEUJeHEQ9ithKuCdfRRapX3vPUvesAdATWb7p98aiIQZTF948hZd6AUo9uFJ0+OAHhHYDhZLcrCg==~3487798~3487285; user_pref_permanentId=6722408; user_pref_givenName=Jack; user_pref_uid=fzaw2008@163.com; acceleratorSecureGUID=03e681c1d39a56647772cce572deb760782e18fa; auth_session=eyJhbGciOiAiZGlyIiwgImVuYyI6ICJBMjU2R0NNIn0..DIxc6YglLdAiqO_r.AzaVyhl6x0oq6qcR8dmsMH3kIXxCn1MZgwbzYCTYwgcCZC2YOmrqdpYegpnU5xeVQhfHMV0OoNWOt1URp-kKzbTcORmX2EABri2VMgFmMzDx6BlYk6VsRjStP-qg6HSrQ_tBZBo_5xXk_fVmyiYBx36AjInNQaSfCzUfFYLN0jNx31Qi39AvCY9bteKrbSDin_FhyW49vbCkDWi5M98KElW5NnSHameS1Dnk3VNp6SviM5AvlFDHkURrd5-tUcvBiK4-ybjDDPleOQRMwPiItXeVW2j5U8hkW2Pvl9JQTZJH61opqmoy8bJttyaRFkW9RVvocyXb29Qbb5SWQ7NSBSyypNNzARUBx3jlEm6lUCL_NpYLxUNFIE3hn7uLqaedRGulVj7saSlbcE2lzb645byK9KT7ir3bAWmpz4RVU7MquWKJJn1YrC0WNRk4KniAMMqmEV3ckuAh3SG9vfAemoms6n02wRQYOnJ54Qj4VfhdoYH_G_lEcBLlqEk5E4IuEi5OzRIXe65obEfk5OxRrEGiyXIOqbB955NRul3D02vQ87IeLYsai4l5VQNo-2bkuxrmgxl4_sqJHq_MxEbi9JcP8JgQmurcSzwPtnLqcjBvVoMI2J-Cg8b6DxUOweN2q2FqOZnGCkuL9NbETPA8QAZtO5hP8VMz1KHUYWtxfCjwnmkHvRtUGbdxMCKF7L3WZ4Xgg2XVBOA0uuC31bI-RuVca_KsEX4MjTsDO96hcZNOZauFwNg7Gl9GDKH0jfJ7pl9SBGOEF2-FjueLdd216g7reBMGRN8q-d3aKzSxB_lYycmYt0WSNXac7Z6kntxviffoFvVdcRYi4SXXy_6YCLmZ9hGUFuXo08-7kUGB9WYLYmWxVQoqD9L3Q9HE8rnqEh6l7yt7ofAWrGs7GVVmbHGDdR0yVYd_IfWCv2TV4mlJtzi49Qsf_6sjDCdZ8HX_jsVi0-e0wQX8J2K8I9RDYh-wcyyORdLf2LUScWDSEsMsfqvKlAtPvV4N8HhC26K23GHyLcJeYtdl7yXwBeVPltnnr_9a5ySmKtMUlX736HNjp7cEHiESrupI9YWBpsXm2t631GsZX-E43U4APX59biCG3ANHxrJbU9hpIlwfTFtYLiyiGRIYquYFd7jzbttO4GAnfXhCMScPwPcnHv2yGJ2RLzIIWwgK0pRzgHvY8_BxaSQLgtcLqgyVjY6Z68aFF1Es1Eavy4YXXHf4CcB5Iy5_zQCjVYsIj6-FDGUwy6KLXrmxm9H6hm8fYL1Nf8gF3SpAPbpQPM-TZxfuyR2A0HNFC7v1pPhRSzkKj3_R3noyIv2zZ_QQFONhze2FoqvLwYfr5RdycBGV0LxnDamVJkhCbkV8Kldpcr6Rj-uqc8hxW_viT-cEcYVHDBOmB4vuT5pgjdSJ5Q-dq7WKuSbp-VKfdZ4rH2UC5cKcE-U0c4f_4R5ivB2VJs1yDqa6NA-IfKe3KsZSCPlzP8-5tUc3cgOKgWycrAECPCOHP59gZYGugERqTtPX2djux1O1cuGnit9ktxUelvNmTcLbX8XwT-tS5YPyCQ_J8QI9KjZHJkQIjaLsDa6cLGQqedJhaV-ayQxawxHzAmGrPek-hPM25Pa6mHPePyixRwNth-3VfcgdyWpbe5jNvKHLO6ew8BHX023MGubvuQFvdxOgMBlzIyO_pMf5kPvNyiXcN42V40FlugSfGQEol_7yIWEWvEIULJmCU9bL.oc5mhKxjt1N3rUi5DDuX5g; tipage=%2Ftistore%2Fcheckout%2Fpayment%20zh; tipageshort=checkout%2Fpayment%20zh; ga_page_cookie=checkout%2Fpayment%20zh; store_CONSENTMGR=ts%3A1629169682990%257Cconsent%3Atrue; store_ti_bm=; _abck=FD295AB47A62AEF2E6FBFE0DB722EB4C~0~YAAQVX+GfB7uzgJ7AQAA59QmUgavkbvIR1V77nXfI40q376oOaM6+DDSQwAhPKXMPF9t5vsOwB8xGFJrrS7MuhiRwrZ21GHgRDOJ0GZxkkM9RZil2augKZWS6z0dfareXqzUzn5MhI6mEUb5Nsvkw1S2UzTmTuM5mLRjAykpPfBJM35Z6jV3Y2SSyNPhZXja610iigKvFO5aLZKeTFnf2bmn5qbGx1VkhEWA6uggP7Tg05F+VLVUkFaNy/5TyaXFnX3qvCeue7qPGUnmMF7qEIO5UbPJzz/AVOVcdAILiKSJ1Sca49kUr0K9jDyUdfbvx6nxuHORB15MiwPw8AsbOiZJ0hH4IONuetw9Geg0tHhhhrqXDn4/CVwFAz/mjtnqOBmp1hBEgdlngmg408k41EfK4B/VLNc=~-1~||-1||~-1; ABTasty=uid=ggz1v697wa96kw8g&fst=1629169684189&pst=-1&cst=1629169684189&ns=1&pvt=12&pvis=12&th=686831.851794.8.8.1.1.1629170147694.1629170926954.1; ABTastySession=mrasn=&sen=22&lp=https%253A%252F%252Fwww.ti.com.cn%252F; ti_rid=2a37caed; ti_bm=Unknown%20Bot%20(3F79075490CF113B9040484F78EC254E)%3amonitor%3a%3aJavaScript%20Fingerprint%20Anomaly%3a%3a; userType=Anonymous; _gat_ga_main_tracker=1; utag_main=v_id:017b52148a340021f565e87cac44030710016069008c7$_sn:1$_ss:0$_pn:12%3Bexp-session$_st:1629172838260$ses_id:1629169682996%3Bexp-session$free_trial:false' \
#   --data-raw 'CSRFToken=97971ba3-4b66-4e89-861f-e9accdb76ac6' \
#   --compressed
# 获取登录地址：
# login.sh
[ $# -eq 1 ] && {
    searchTerm=$1
    tmp=$(curl 'https://www.ti.com.cn/search/gpn?searchTerm='$searchTerm'&locale=zh-CN' --compressed -s | jq ".gpn,.opns[]")
    gpn=${tmp[0]}
    echo $gpn
    
    
    exit 0
}
#模糊匹配
[ $# -eq 2 -a "$2" == "m" ] && {
    searchTerm=$1
    curl 'https://www.ti.com.cn/search/partMatch?searchTerm='$searchTerm'&locale=zh-CN' --compressed -s | jq  ".[].partNumber" | xargs -i $0 {}
}
echo "$0 搜索产品的型号"
