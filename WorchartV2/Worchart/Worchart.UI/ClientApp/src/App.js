import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FirmHome } from './components/FirmHome';
import { FirmTeam } from './components/FirmTeam';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import { Login } from './components/Login';
import { Register } from './components/Register';

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route exact path='/firma' component={FirmHome} />
                <Route path='/firma/ekip/:firmid' component={FirmTeam} />
                <Route path='/counter' component={Counter} />
                <Route path='/fetch-data' component={FetchData} />
                <Route path='/login' component={Login} />
                <Route path='/uye-ol' component={Register} />
                <Route path='/user/:userid' component={Login} />
            </Layout>
        );
    }
}