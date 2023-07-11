import { WorchartDispatcher } from "./WorchartDispatcher";

export function setCompany(companyData) {
    WorchartDispatcher.dispatch({
        type: "set_company",
        data: companyData
    });
}

export function saveTeam(team, isNew) {
    WorchartDispatcher.dispatch({
        type: "save_team",
        data: { team, new: isNew}
    });
}