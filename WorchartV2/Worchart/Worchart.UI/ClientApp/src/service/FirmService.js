import BaseService from './BaseService';
import WorchartStore from "../falanx/WorchartStore";
import * as FirmActions from "../falanx/FirmActions";

class FirmService extends BaseService {
    async getCompany(companyId) {
        return await this.getCompanyView(companyId);
    }

    async getTeams(firmId) {
        var model = await this.getCompanyView(firmId);
        return model.teamList;
    }

    async getCompanyView(companyId) {
        var company = WorchartStore.getCompany();
        if (!company || company.id !== companyId) {
            company = await super.getData("/company/detail/" + companyId);
            console.log("OS3", company);
            FirmActions.setCompany(company);
        }
        return company;
    }

    async saveTeam(firmId, team) {
        if (firmId > 0 && team) {
            team.companyId = firmId;
            team.id = team.id || 0;
            var isNew = team.id == 0;

            team = await super.postData("/company/team/", team);
            FirmActions.saveTeam(team, isNew);
            return team;
        }
        return undefined;
    }
}

export default FirmService = new FirmService();