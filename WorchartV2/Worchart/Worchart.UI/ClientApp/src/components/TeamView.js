import React, { useState, useEffect } from 'react';
import FirmService from '../service/FirmService';
import WForm from '../controls/WForm';
import RS from '../service/LanguageMapService';

function TeamView(props) {
    var [editable, setEditable] = useState(props.team.mode === "edit");
    var [team, setTeam] = useState(props.team);
    var [teamUsers, setUsers] = useState(props.team.userList);

   var form = {
        id: "team",
        model: props.team,
        items: [
            { type: "text", title: "LBL.TEAMNAME", field: "name", maxLength: 50, required: true },
            { type: "text", title: "LBL.TEAMSHORTNAME", field: "shortName", maxLength: 20, required: true },
            { type: "textarea", title: "LBL.DESCRIPTION", field: "description", maxLength: 200 },
            { type: "file", title: "LBL.TEAMLOGO", field: "logo" }
        ]
    };

    useEffect(() => {
        setUsers(props.team.userList);
        setTeam(props.team);
        setEditable(props.team.mode === "edit");
        form.model = props.team;
    }, [props.team]);

    function saveTeam(model){
        console.log(model);
        FirmService.saveTeam(team.companyID, model).then(result => {
            console.log(result);
            if (result) {
                FirmService.showSuccess("MSG.COMMON.SAVECOMPLETED");
                this.loadTeams(true);
            }
            else {
                return "MSG.TEAM.SAVEFAILED";
            }
        });
    }

    function getTeamUsers() {
        if (!teamUsers || teamUsers.length == 0) {
            return <div className="row user">
                <p className="lead">{RS.get("MSG.TEAM.NOUSERADDED")}</p>
            </div>
        }

        return teamUsers.map(u => {
            return <div className="row user">
                <div className="col-md-2">
                    <img src={`/res/img/user/0.jpg`} alt={u.name} className="img-fluid" />
                </div>
                <div className="col-md-6">
                    <span className="name">{u.name}</span><br />
                    <span>{u.email}</span><br />
                    <span className="user-role">{u.role ? u.role.value : "-"}</span>
                </div>
                <div className="col-md-4">
                    <span>
                        <i className="fa fa-bug"></i>
                        <i className="fa fa-cog"></i>
                    </span>
                </div>
            </div>
        });
    }

    return (
        <div className="row team-view">
            <div className="col col-md-4">
                <div className="img">
                    <img src={`/res/img/${team.logo || "team/0.jpg"}`} alt={team.name} className="img-fluid" />
                </div>
                <h2 className="title"><a href="#">{team.name}</a></h2>
                <p>{team.description}</p>
            </div>
            {
                !editable && <div className="col col-md-8">
                    <h3>{RS.get("TITLE.TEAMUSERS")}</h3>
                    <div className="users-panel">{getTeamUsers()}</div>
                </div>
            }
            {
                editable && <div className="col col-md-8">
                    <WForm title="LBL.SAVETEAM" form={form} className="newTeamForm" onSubmit={(form) => saveTeam(form)} />
                </div> 
            }
        </div>
    );
}

export default TeamView;