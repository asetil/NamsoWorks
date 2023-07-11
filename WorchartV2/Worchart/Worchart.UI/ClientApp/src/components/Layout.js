import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import Header from './Header';
import { WDialog } from '../controls/WDialog';

export class Layout extends Component {
    static displayName = Layout.name;

    render() {
        return (
            <React.Fragment>
                <Header />
                <main id="main">
                    <section id="content">
                        <div>
                            {this.props.children}
                        </div>
                    </section>
                </main>
                <WDialog/>
            </React.Fragment>
        );
    }
}
