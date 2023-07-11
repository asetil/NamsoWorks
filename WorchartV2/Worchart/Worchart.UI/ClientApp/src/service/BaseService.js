import Helper from "./ServiceHelper"
import * as DialogActions from "../falanx/DialogActions";
import RS from '../service/LanguageMapService';
import WorchartStore from "../falanx/WorchartStore";

export default class BaseService {
    showSuccess(message, title) {
        title = title || "MSG.COMMONSUCCESS.TITLE";
        message = message || "MSG.COMMONSUCCESS";
        var item = { id: 0, title: RS.get(title), message: message, type: DialogActions.ToasterTypes.Success };
        DialogActions.addToaster(item, 4000);
    }

    showError(message, title) {
        title = title || "MSG.COMMONERROR.TITLE";
        message = message || "MSG.COMMONERROR";
        var item = { id: 0, title: RS.get(title), message: message, type: DialogActions.ToasterTypes.Error };
        DialogActions.addToaster(item, 4000);
    }

    async getData(url) {
        return this.fetchData(url, 'GET');
    }

    async putData(url, data) {
        var header = await this.getHeader(url);
        return fetch(Helper.baseUrl + url, {
            method: 'PUT',
            headers: header,
            //credentials: "include",
            body: JSON.stringify(data)
        }).then(response => {
            return response.json();
        }).then(async jsonData => {
            if (!jsonData.success) {
                var errorResponse = await this.handleError(jsonData, this.postData, [url, data]);
                return errorResponse;
            }
            return jsonData.value;
        });
    }

    async postData(url, data) {
        var header = await this.getHeader(url);
        return fetch(Helper.baseUrl + url, {
            method: 'POST',
            headers: header,
            //credentials: "include",
            body: JSON.stringify(data)
        }).then(response => {
            return response.json();
        }).then(async jsonData => {
            if (!jsonData.success) {
                var errorResponse = await this.handleError(jsonData, this.postData, [url, data]);
                return errorResponse;
            }
            return jsonData.value;
        });
    }

    async fetchData(url, methodType) {
        var header = await this.getHeader(url);
        var data = {
            method: methodType,
            headers: header,
            //credentials: 'include'
        };

        return fetch(Helper.baseUrl + url, data).then(response => {
            return response.json();
        }).then(async jsonData => {
            if (!jsonData.success) {
                var errorResponse = await this.handleError(jsonData, this.fetchData, [url, methodType]);
                return errorResponse;
            }
            return jsonData.value;
        });
    }

    async handleError(response, callback, params) {
        if (Helper.tokenCount < 1 && response.code == Helper.invalidAccessTokenMsg) {
            console.log("ossi", "will refresh access token");
            var accessToken = await this.refreshToken();

            Helper.tokenCount++;
            if (callback) {
                callback(params);
            }
            return;
        }

        Helper.tokenCount = 0;
        var message = response.message || RS.get("MSG.COMMONERROR");
        this.showError(message, "MSG.COMMONERROR.TITLE");
        return undefined;
    }

    async getHeader(url) {
        var accessToken = "";
        if (url !== "/token/authorize" && url !== "/token/accesstoken" && url !== "/token/refresh") {
            accessToken = await this.getAccessToken();
        }

        var userToken = WorchartStore.getUserToken();
        var result = {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'AccessToken': accessToken,
            'CustomerToken': userToken,
            'lang': Helper.currentLanguage
        };
        return result;
    }

    async getAccessToken() {
        if (!Helper.accessToken) {
            var data = {
                clientID: Helper.clientId,
                apiKey: Helper.apiKey,
            };

            var response = await this.postData('/token/accesstoken', data);
            if (response && response.token) {
                Helper.accessToken = response.token;
                Helper.refreshToken = response.refreshToken;
            }
            else {
                throw 'FATAL : Access token failure!';
            }
        }
        return Helper.accessToken;
    }

    async refreshToken() {
        Helper.accessToken = "";
        var data = {
            clientID: Helper.clientId,
            apiKey: Helper.apiKey,
            authorizeToken: Helper.refreshToken
        };

        var response = await this.postData('/token/refresh', data);
        if (response && response.token) {
            Helper.accessToken = response.token;
            Helper.refreshToken = response.refreshToken;
        }
        else {
            throw 'FATAL : Refresh token failure!';
        }
        return Helper.accessToken;
    }

    //async refreshToken() {
    //    var data = {
    //        clientID: Helper.clientId,
    //        authorizeToken: Helper.refreshToken,
    //        requestTime: new Date().toISOString(),
    //        tokenHash: ""
    //    };

    //    var hashStr = Helper.refreshToken + ":" + data.clientID + ":" + Helper.clientSecret + ":" + data.requestTime;
    //    data.tokenHash = this.SHA1(hashStr);

    //    var response = await this.postData('/token/refresh', data);
    //    Helper.accessToken = response.token;
    //    Helper.refreshToken = response.refreshToken;
    //    return Helper.accessToken;
    //}

    //async getAccessToken() {
    //    if (!Helper.accessToken) {
    //        var authorizeToken = await this.getAuthorizeToken();

    //        var data = {
    //            clientID: Helper.clientId,
    //            authorizeToken,
    //            requestTime: new Date().toISOString(),
    //            tokenHash: ""
    //        };

    //        var hashStr = authorizeToken + ":" + data.clientID + ":" + Helper.clientSecret + ":" + data.requestTime;
    //        data.tokenHash = this.SHA1(hashStr);

    //        var response = await this.postData('/token/accesstoken', data);
    //        Helper.accessToken = response.token;
    //        Helper.refreshToken = response.refreshToken;
    //    }
    //    return Helper.accessToken;
    //}

    //async getAuthorizeToken() {
    //    var data = {
    //        clientID: Helper.clientId,
    //        requestTime: new Date().toISOString(),
    //        tokenHash: ""
    //    };

    //    var hashStr = data.clientID + ":" + Helper.clientSecret + ":" + data.requestTime;
    //    data.tokenHash = this.SHA1(hashStr);
    //    var response = await this.postData("/token/authorize", data);
    //    return response.token;
    //}


    //getSha1(str) {
    //    //  discuss at: http://phpjs.org/functions/sha1/
    //    // original by: Webtoolkit.info (http://www.webtoolkit.info/)
    //    // improved by: Michael White (http://getsprink.com)
    //    // improved by: Kevin van Zonneveld (http://kevin.vanzonneveld.net)
    //    //    input by: Brett Zamir (http://brett-zamir.me)
    //    //  depends on: utf8_encode
    //    //   example 1: sha1('Kevin van Zonneveld');
    //    //   returns 1: '54916d2e62f65b3afa6e192e6a601cdbe5cb5897'

    //    var rotate_left = function (n, s) {
    //        var t4 = (n << s) | (n >>> (32 - s));
    //        return t4;
    //    };

    //    /*var lsb_hex = function (val) { // Not in use; needed?
    //      var str="";
    //      var i;
    //      var vh;
    //      var vl;

    //      for ( i=0; i<=6; i+=2 ) {
    //        vh = (val>>>(i*4+4))&0x0f;
    //        vl = (val>>>(i*4))&0x0f;
    //        str += vh.toString(16) + vl.toString(16);
    //      }
    //      return str;
    //    };*/

    //    var cvt_hex = function (val) {
    //        var str = '';
    //        var i;
    //        var v;

    //        for (i = 7; i >= 0; i--) {
    //            v = (val >>> (i * 4)) & 0x0f;
    //            str += v.toString(16);
    //        }
    //        return str;
    //    };

    //    var blockstart;
    //    var i, j;
    //    var W = new Array(80);
    //    var H0 = 0x67452301;
    //    var H1 = 0xEFCDAB89;
    //    var H2 = 0x98BADCFE;
    //    var H3 = 0x10325476;
    //    var H4 = 0xC3D2E1F0;
    //    var A, B, C, D, E;
    //    var temp;

    //    str = this.utf8_encode(str);
    //    var str_len = str.length;

    //    var word_array = [];
    //    for (i = 0; i < str_len - 3; i += 4) {
    //        j = str.charCodeAt(i) << 24 | str.charCodeAt(i + 1) << 16 | str.charCodeAt(i + 2) << 8 | str.charCodeAt(i + 3);
    //        word_array.push(j);
    //    }

    //    switch (str_len % 4) {
    //        case 0:
    //            i = 0x080000000;
    //            break;
    //        case 1:
    //            i = str.charCodeAt(str_len - 1) << 24 | 0x0800000;
    //            break;
    //        case 2:
    //            i = str.charCodeAt(str_len - 2) << 24 | str.charCodeAt(str_len - 1) << 16 | 0x08000;
    //            break;
    //        case 3:
    //            i = str.charCodeAt(str_len - 3) << 24 | str.charCodeAt(str_len - 2) << 16 | str.charCodeAt(str_len - 1) <<
    //                8 | 0x80;
    //            break;
    //    }

    //    word_array.push(i);

    //    while ((word_array.length % 16) != 14) {
    //        word_array.push(0);
    //    }

    //    word_array.push(str_len >>> 29);
    //    word_array.push((str_len << 3) & 0x0ffffffff);

    //    for (blockstart = 0; blockstart < word_array.length; blockstart += 16) {
    //        for (i = 0; i < 16; i++) {
    //            W[i] = word_array[blockstart + i];
    //        }
    //        for (i = 16; i <= 79; i++) {
    //            W[i] = rotate_left(W[i - 3] ^ W[i - 8] ^ W[i - 14] ^ W[i - 16], 1);
    //        }

    //        A = H0;
    //        B = H1;
    //        C = H2;
    //        D = H3;
    //        E = H4;

    //        for (i = 0; i <= 19; i++) {
    //            temp = (rotate_left(A, 5) + ((B & C) | (~B & D)) + E + W[i] + 0x5A827999) & 0x0ffffffff;
    //            E = D;
    //            D = C;
    //            C = rotate_left(B, 30);
    //            B = A;
    //            A = temp;
    //        }

    //        for (i = 20; i <= 39; i++) {
    //            temp = (rotate_left(A, 5) + (B ^ C ^ D) + E + W[i] + 0x6ED9EBA1) & 0x0ffffffff;
    //            E = D;
    //            D = C;
    //            C = rotate_left(B, 30);
    //            B = A;
    //            A = temp;
    //        }

    //        for (i = 40; i <= 59; i++) {
    //            temp = (rotate_left(A, 5) + ((B & C) | (B & D) | (C & D)) + E + W[i] + 0x8F1BBCDC) & 0x0ffffffff;
    //            E = D;
    //            D = C;
    //            C = rotate_left(B, 30);
    //            B = A;
    //            A = temp;
    //        }

    //        for (i = 60; i <= 79; i++) {
    //            temp = (rotate_left(A, 5) + (B ^ C ^ D) + E + W[i] + 0xCA62C1D6) & 0x0ffffffff;
    //            E = D;
    //            D = C;
    //            C = rotate_left(B, 30);
    //            B = A;
    //            A = temp;
    //        }

    //        H0 = (H0 + A) & 0x0ffffffff;
    //        H1 = (H1 + B) & 0x0ffffffff;
    //        H2 = (H2 + C) & 0x0ffffffff;
    //        H3 = (H3 + D) & 0x0ffffffff;
    //        H4 = (H4 + E) & 0x0ffffffff;
    //    }

    //    temp = cvt_hex(H0) + cvt_hex(H1) + cvt_hex(H2) + cvt_hex(H3) + cvt_hex(H4);
    //    return temp.toLowerCase();
    //}

    /**
    * Secure Hash Algorithm (SHA1)
    * http://www.webtoolkit.info/
    **/
    SHA1(msg) {
        function rotate_left(n, s) {
            var t4 = (n << s) | (n >>> (32 - s));
            return t4;
        };
        function lsb_hex(val) {
            var str = '';
            var i;
            var vh;
            var vl;
            for (i = 0; i <= 6; i += 2) {
                vh = (val >>> (i * 4 + 4)) & 0x0f;
                vl = (val >>> (i * 4)) & 0x0f;
                str += vh.toString(16) + vl.toString(16);
            }
            return str;
        };
        function cvt_hex(val) {
            var str = '';
            var i;
            var v;
            for (i = 7; i >= 0; i--) {
                v = (val >>> (i * 4)) & 0x0f;
                str += v.toString(16);
            }
            return str;
        };
        function Utf8Encode(string) {
            string = string.replace(/\r\n/g, '\n');
            var utftext = '';
            for (var n = 0; n < string.length; n++) {
                var c = string.charCodeAt(n);
                if (c < 128) {
                    utftext += String.fromCharCode(c);
                }
                else if ((c > 127) && (c < 2048)) {
                    utftext += String.fromCharCode((c >> 6) | 192);
                    utftext += String.fromCharCode((c & 63) | 128);
                }
                else {
                    utftext += String.fromCharCode((c >> 12) | 224);
                    utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                    utftext += String.fromCharCode((c & 63) | 128);
                }
            }
            return utftext;
        };
        var blockstart;
        var i, j;
        var W = new Array(80);
        var H0 = 0x67452301;
        var H1 = 0xEFCDAB89;
        var H2 = 0x98BADCFE;
        var H3 = 0x10325476;
        var H4 = 0xC3D2E1F0;
        var A, B, C, D, E;
        var temp;
        msg = Utf8Encode(msg);
        var msg_len = msg.length;
        var word_array = new Array();
        for (i = 0; i < msg_len - 3; i += 4) {
            j = msg.charCodeAt(i) << 24 | msg.charCodeAt(i + 1) << 16 |
                msg.charCodeAt(i + 2) << 8 | msg.charCodeAt(i + 3);
            word_array.push(j);
        }
        switch (msg_len % 4) {
            case 0:
                i = 0x080000000;
                break;
            case 1:
                i = msg.charCodeAt(msg_len - 1) << 24 | 0x0800000;
                break;
            case 2:
                i = msg.charCodeAt(msg_len - 2) << 24 | msg.charCodeAt(msg_len - 1) << 16 | 0x08000;
                break;
            case 3:
                i = msg.charCodeAt(msg_len - 3) << 24 | msg.charCodeAt(msg_len - 2) << 16 | msg.charCodeAt(msg_len - 1) << 8 | 0x80;
                break;
        }
        word_array.push(i);
        while ((word_array.length % 16) != 14) word_array.push(0);
        word_array.push(msg_len >>> 29);
        word_array.push((msg_len << 3) & 0x0ffffffff);
        for (blockstart = 0; blockstart < word_array.length; blockstart += 16) {
            for (i = 0; i < 16; i++) W[i] = word_array[blockstart + i];
            for (i = 16; i <= 79; i++) W[i] = rotate_left(W[i - 3] ^ W[i - 8] ^ W[i - 14] ^ W[i - 16], 1);
            A = H0;
            B = H1;
            C = H2;
            D = H3;
            E = H4;
            for (i = 0; i <= 19; i++) {
                temp = (rotate_left(A, 5) + ((B & C) | (~B & D)) + E + W[i] + 0x5A827999) & 0x0ffffffff;
                E = D;
                D = C;
                C = rotate_left(B, 30);
                B = A;
                A = temp;
            }
            for (i = 20; i <= 39; i++) {
                temp = (rotate_left(A, 5) + (B ^ C ^ D) + E + W[i] + 0x6ED9EBA1) & 0x0ffffffff;
                E = D;
                D = C;
                C = rotate_left(B, 30);
                B = A;
                A = temp;
            }
            for (i = 40; i <= 59; i++) {
                temp = (rotate_left(A, 5) + ((B & C) | (B & D) | (C & D)) + E + W[i] + 0x8F1BBCDC) & 0x0ffffffff;
                E = D;
                D = C;
                C = rotate_left(B, 30);
                B = A;
                A = temp;
            }
            for (i = 60; i <= 79; i++) {
                temp = (rotate_left(A, 5) + (B ^ C ^ D) + E + W[i] + 0xCA62C1D6) & 0x0ffffffff;
                E = D;
                D = C;
                C = rotate_left(B, 30);
                B = A;
                A = temp;
            }
            H0 = (H0 + A) & 0x0ffffffff;
            H1 = (H1 + B) & 0x0ffffffff;
            H2 = (H2 + C) & 0x0ffffffff;
            H3 = (H3 + D) & 0x0ffffffff;
            H4 = (H4 + E) & 0x0ffffffff;
        }
        var temp = cvt_hex(H0) + cvt_hex(H1) + cvt_hex(H2) + cvt_hex(H3) + cvt_hex(H4);

        return temp.toUpperCase();
    }
}