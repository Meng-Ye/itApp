
var domains = [".ti.com", "www.ti.com.cn", ".ti.com.cn", "www.ti.com"];
console.log("hook is loaded", new Date());
function addJquery() {
    if (typeof jQuery == 'undefined') {
        var s = document.createElement('script');
        s.src = 'https://libs.baidu.com/jquery/2.0.3/jquery.min.js';
        document.getElementsByTagName('body')[0].appendChild(s);
        return false;
    }
    return true;
}
//设置
function setCookie() {
    addJquery();
    for (var i = 0; i < domains.length; i++) {
        document.cookie = "user_pref_shipTo=\"CN\";path=/;domain=" + domains[i];
        document.cookie = "user_pref_language=\"zh-CN\";path=/;domain=" + domains[i];
        document.cookie = "user_pref_currency=\"CNY\";path=/;domain=" + domains[i];
    }
}
//登录
//https://www.ti.com.cn/secure-link-forward/?gotoUrl=https%3A%2F%2Fwww.ti.com.cn%2Fproduct%2Fcn%2FLM10
function login(username, password) {
    if (!username) {
        console.log("登录用户名呢？");
    }
    if (!password) {
        console.log("登录用密码呢？");
    }
    setCookie();
    $.post(document.forms[0].action, "pf.username=" + username + "&pf.pass=" + password + "&pf.adapterId=IDPAdapterHTMLFormCIDStandard", function (d) {
        idx = d.indexOf("<form");
        end = d.indexOf("</form");
        d = d.substring(idx, end + 7);
        document.write(d);
        document.forms[0].submit()
    })
}
var pageData, tmpOffers, currencyExchangeRate, storeCustomReelFee;
function hasTmpOffers() {
    $("script").each((idx, el) => {
        if (el.type == "application/ld+json")
            pageData = JSON.parse($(el).text());
    });
    if (pageData) {
        var offers = pageData.offers;
        for (var i = 0; i < offers.length; i++) {
            if (parseInt(offers[i].itemOffered.offers.inventoryLevel) > 0) {
                if (!tmpOffers) {
                    tmpOffers = offers[i];
                } else {
                    if (parseFloat(tmpOffers.price) > parseFloat(offers[i].price)) {
                        tmpOffers = offers[i];
                    }
                }
            }
        }
        var additionalProperty = pageData.additionalProperty;
        for (var i = 0; i < additionalProperty.length; i++) {
            if (additionalProperty[i].name == "currencyExchangeRate") {
                currencyExchangeRate = additionalProperty[i].valueReference;
            } else if (additionalProperty[i].name == "storeCustomReelFee") {
                storeCustomReelFee = additionalProperty[i].valueReference;
            }
        }
        if (tmpOffers) {
            return { tmpOffers, currencyExchangeRate, storeCustomReelFee };
        }
    }
    console.log("no pageData");
    return null;
}

function getBuyCount(priceCurrency, price, cnyRace, buyMonyey, inventoryLevel) {
    var singlePrice = 1;
    if (priceCurrency == "CNY") {
        singlePrice = parseFloat(price);
    } else {
        singlePrice = parseFloat(price) * cnyRace;
    }
    if (singlePrice == 0) {
        return 1;
    }
    buyCount = parseInt(buyMonyey / singlePrice);
    if (isNaN(buyCount) || buyCount < 0) {
        buyCount = 1;
    }
    if (buyCount > inventoryLevel) {
        buyCount = inventoryLevel;
    }
    return { buyCount, singlePrice, inventoryLevel };
}

//加入购物车并提交
function order(buyMonyey) {
    setCookie();
    var cnyRace = 6.38;
    var data;
    var addToCartForm = $("#addToCartForm");
    var buyCountInfo;
    if (addToCartForm.length == 0) {
        var ndata = hasTmpOffers();
        if (ndata == null) {
            console.log("找不到数据", new Date());
            return false;
        }
        if (typeof currencyExchangeRate != "undefined") {
            currencyExchangeRate.forEach(item => {
                if (item.name == "CNY") {
                    cnyRace = parseFloat(item.value)
                }
            });
        }


        //tmpOffers.itemOffered.offers.priceSpecification 区间价格
        if (tmpOffers.itemOffered.offers.priceSpecification) {
            for (var i = tmpOffers.itemOffered.offers.priceSpecification.length - 1; i >= 0; i--) {
                var priceSpecification = tmpOffers.itemOffered.offers.priceSpecification[i];
                buyCountInfo = getBuyCount(priceSpecification.priceCurrency, priceSpecification.price, cnyRace, buyMonyey, tmpOffers.itemOffered.offers.inventoryLevel);
                if (buyCountInfo.buyCount >= priceSpecification.eligibleQuantity.minValue && buyCountInfo.buyCount <= priceSpecification.eligibleQuantity.maxValue) {
                    break;
                }
            }
        } else {
            buyCountInfo = getBuyCount(tmpOffers.itemOffered.priceCurrency, tmpOffers.itemOffered.price, cnyRace, buyMonyey, tmpOffers.itemOffered.offers.inventoryLevel);
        }
        buyCountInfo.prodName = tmpOffers.itemOffered.name;
        data = { cartRequestList: [{ opnId: tmpOffers.itemOffered.name, packageOption: null, quantity: buyCountInfo.buyCount, sparam: "", tiAddtoCartSource: "ti.com-productfolder" }], currency: "CNY" };
    } else {
        //库存
        var inventoryLevel = $("#addToCartForm").find("input[name=pdpAvailQty]").val();
        var productCodePost = $("#addToCartForm").find("input[name=productCodePost]").val();
        var numberCells = $(".pdp-price-table").find("td.number-cell");
        var priceSpecification = [];
        for (var i = 0; i < numberCells.length; i++) {
            var td = $(numberCells[i]);
            var price = td.text().trim();
            var values = td.prev().text().trim().split("-");
            var currency = td.attr("data-currency");
            if (!currency) {
                currency = "CNY";
            }
            priceSpecification.push({ minValue: values[0], maxValue: values[1], price: price, currency: currency });
        }
        for (var i = priceSpecification.length - 1; i >= 0; i--) {
            var priceSpec = priceSpecification[i];
            var price = priceSpec.price;
            if (price[0] == "¥") {
                price = price.substring(1);
            }
            buyCountInfo = getBuyCount(priceSpec.currency, price, cnyRace, buyMonyey, inventoryLevel);
            if (buyCountInfo.buyCount >= priceSpec.minValue && buyCountInfo.buyCount <= priceSpec.maxValue) {
                break;
            }
        }
        buyCountInfo.prodName = productCodePost;
        data = { cartRequestList: [{ opnId: productCodePost, packageOption: "CTX", quantity: buyCountInfo.buyCount, sparam: "", tiAddtoCartSource: "store-pdp" }], currency: "CNY" };
    }
    //加入购物车
    var cardData;
    $.ajax('/recaptchaproxy/occservices/v2/ti/addtocart', {
        method: 'POST',
        async: false,
        contentType: 'application/json;charset=utf-8',
        data: JSON.stringify(data),
        success: function (datas) {
            cardData = datas;
        }, error: function (e) {
            cardData = false;
            console.log(e);
        }
    })
    if (!cardData) {
        return false;
    }
    console.log(data, cardData, buyCountInfo);
    setTimeout(() => {
        location.href = "https://www.ti.com.cn/samlsinglesignon/saml/alias/ticn/?site=ti&samlPage=cart&dotcomCartId=" + cardData.cartId + "&contShopUrl=" + encodeURIComponent(location.href);
    }, 100);
    return { cardData, buyCountInfo };
}
//检查购物车，删除不要的
function checkCart(prodName, quantity) {
    var entrys = $.find("a[id^=actionEntry_]");
    for (var i = 0; i < entrys.length; i++) {
        var entry = entrys[i];
        if ($(entry).attr("data-entry-product-code") == prodName) {
            if ($(entry).attr("data-entry-initial-quantity") != quantity) {
                updateData = {};
                var form = $("#updateCartForm" + i)[0];
                updateData.entryNumber = form.entryNumber.value;
                updateData.productCode = form.productCode.value;
                updateData.initialQuantity = quantity;
                updateData.carrierOption = form.carrierOption.value;
                updateData.quantity = quantity;
                updateData.CSRFToken = form.CSRFToken.value;
                $.ajax('/store/ti/zh/cart/update', {
                    method: 'POST',
                    async: false,
                    data: updateData,
                    success: function (datas) {

                    }
                })

            }

        } else {
            $.ajax('/store/ti/zh/cart/entry/execute/REMOVE?entryNumbers=' + i, {
                method: 'GET',
                async: false,
                success: function (datas) {

                }
            })
        }
    }
    setTimeout(() => {
        location.href = "https://www.ti.com.cn/store/ti/zh/cart/checkout";
    }, 100)
    return true;
}
//地址
function deliveryAddress() {
    setCookie();
    data = {
        addressType: "Business",
        countryIso: "CN",
        editAddress: "true",
        addressId: "8986547978263",
        shippingAddress: true,
        firstName: "Justin",
        lastName: "Wang",
        line1: "jishsaa fasdf adfas",
        line2: "A1215,Hurun Mingyuan,Fuchen",
        townCity: "shenzhen longhua",
        state: "shenzhen guangdong",
        postcode: "518000",
        companyName: "Santech",
        companyUrl: "none",
        phoneCountryPrefix: "86-CN",
        phoneNumber: "861 8138 205458",
        phoneIsMobile: true,
        email: "jimmy.wang@zo-phone.com",
        same_billingAddress: "on",
        saveInAddressBook: false,
        billingAddress: true
    }
    var form = document.getElementById("paid-shipping-address-form");
    $("input[name=addressType][value=Business]").change();
    form.addressType.value = data.addressType;
    form.countryIso.value = data.countryIso;
    form.firstName.value = data.firstName;
    form.lastName.value = data.lastName;
    form.line1.value = data.line1;
    form.line2.value = data.line2;
    form.townCity.value = data.townCity;
    if (form.state[1]) {
        form.state[1].value = data.state;
    } else {
        form.state.value = data.state;
    }
    form.phoneNumber.value = data.phoneNumber;
    form.phoneCountryPrefix.value = data.phoneCountryPrefix;
    form.saveInAddressBook.checked = false;
    form.same_billingAddress.value = data.same_billingAddress;
    form.email.value = data.email;
    form.shippingAddress.checked = true;
    form.postcode.value = data.postcode;
    form.companyName.value = data.companyName;
    form.companyUrl.value = data.companyUrl;
    form.phoneIsMobile.checked = data.phoneIsMobile;
    setTimeout(() => {
        $("#paid-shipping-address-select").removeAttr("disabled").click();
    }, 100);
    return true;
}
//发票
function taxInvoice() {
    setCookie();
    data = {
        type: "SPECIALPAPERINVOICE",
        recipient: "深圳市极点科技有限公司",
        taxRegistrationNumber: "91440300MA5D8NT68H",
        registrationAddress: "深圳市龙华区民治街道新牛社区新牛路港深国际中心A5-03",
        phoneNumber: "133 1698 1558",
        bankName: "中国工商银行深圳梅林支行",
        bankAccountNo: "4000026209200360825",
        "vatInvoiceAddress.firstName": "JACK WANG",
        "vatInvoiceAddress.companyName": "深圳市极点科技有限公司",
        "vatInvoiceAddress.phoneCountryPrefix": "0",
        "vatInvoiceAddress.phoneNumber": "133 1698 1558",
        billTo_countryIso: "0",
        "vatInvoiceAddress.countryIso": "CN",
        "vatInvoiceAddress.line1": "深圳市龙华区民治街道新牛社区新牛路港深国际中心A5-03",
        "vatInvoiceAddress.townCity": "深圳",
        "vatInvoiceAddress.state": "广东",
        "vatInvoiceAddress.postcode": "518000"
    }
    var form = document.getElementById("formInvoice");
    $("#special-invoice").val(data.type).change();
    $("#cmpCheckboxflag").click();
    $("#saveVatInvoice").click();
    form.type.value = data.type;
    form.recipient.value = data.recipient;
    form.taxRegistrationNumber.value = data.taxRegistrationNumber;
    form.registrationAddress.value = data.registrationAddress;
    form.phoneNumber.value = data.phoneNumber;
    form.billTo_countryIso.value = data.billTo_countryIso;
    form.bankName.value = data.bankName;
    form.bankAccountNo.value = data.bankAccountNo;
    form["vatInvoiceAddress.firstName"].value = data["vatInvoiceAddress.firstName"];
    form["vatInvoiceAddress.companyName"].value = data["vatInvoiceAddress.companyName"];
    form["vatInvoiceAddress.phoneCountryPrefix"].value = data["vatInvoiceAddress.phoneCountryPrefix"];
    form["vatInvoiceAddress.phoneNumber"].value = data["vatInvoiceAddress.phoneNumber"];
    form["vatInvoiceAddress.countryIso"].value = data["vatInvoiceAddress.countryIso"];
    form["vatInvoiceAddress.line1"].value = data["vatInvoiceAddress.line1"];
    form["vatInvoiceAddress.townCity"].value = data["vatInvoiceAddress.townCity"];
    form["vatInvoiceAddress.state"].value = data["vatInvoiceAddress.state"];
    form["vatInvoiceAddress.postcode"].value = data["vatInvoiceAddress.postcode"];

    $("#tax-invoice-submit").removeAttr("disabled").click();
    setTimeout(() => {
        $(".modal-default button[type=submit]").click();
    }, 100);
    return true;
}
//类型
function regulations() {
    setCookie();
    var form = document.getElementById("regulations-form");
    var crs = $("#checkout-regulations-select");
    var options = crs.find("option");
    if (options.length <= 1) {
        $("#regulations-select").val("家庭影音&娱乐").change();
        return false;
    }
    var idx = Math.floor(Math.random() * options.length);
    crs.val($(options[idx]).text()).change();
    form.militaryFlag.value = "No";
    form.militaryFlag[0].disabled = false;
    form.militaryFlag[1].disabled = false;
    setTimeout(() => {
        var btn = document.getElementById("regulations-submit-btn");
        btn.disabled = false;
        btn.click();
    }, 100);
    return true;
}

//快递
function deliveryMethod() {
    setCookie();
    var delivery = $("#delivery_method");
    delivery[0].selectedIndex = 0;
    delivery.change();
    $("#terms_accept").click();
    setTimeout(() => {
        location.href = "https://www.ti.com.cn/store/ti/zh/checkout/buy/multi/delivery-method/select?delivery_method=v2-china-INT-EH&termsAccepted=Yes";
    }, 100);
    return true;
}

//支付
function paymentMethod() {
    setCookie();
    $("#payment-method-wechatpay").click();
    setTimeout(() => {
        $("form button:visible").click();
    }, 100);
    return true;
}

//获取订单信息
function orderInfo() {
    if (!addJquery()) {
        console.log("no jquery");
        return false;
    }
    var payAddr = $("iframe").attr("src");
    var tiOrderCode = $("#tiOrderCode").val();
    var orderTotal = $(".order-total").text().trim();
    if (payAddr && tiOrderCode && orderTotal) {
        return { payAddr, tiOrderCode, orderTotal };
    }
    console.log("error data");
    return false;
}