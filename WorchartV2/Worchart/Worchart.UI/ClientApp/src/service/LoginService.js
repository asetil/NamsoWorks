import BaseService from './BaseService';
import WorchartStore from "../falanx/WorchartStore";
import * as LoginActions from "../falanx/LoginActions";

class LoginService extends BaseService {
    async handleLogin(email, password, companyCode) {
        var data = { email, password, companyCode };
        var result = await super.postData("/User/Login", data);

        if (result && result.userToken) {
            LoginActions.setLoggedIn(result);
        }
        return result;
    }

    getLoginState() {
        return WorchartStore.getLoginState();
    }
}

export default LoginService = new LoginService();