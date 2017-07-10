import React from 'react'

import {
    BrowserRouter as Router,
    Route,
    Link,
    Redirect,
    Switch
} from 'react-router-dom'
import { RouteTransition, presets as RouteTransitionPresets } from 'react-router-transition';
import { Nav, NavItem } from 'react-bootstrap'
import { IndexLinkContainer, LinkContainer } from 'react-router-bootstrap'

import './Honest.css';

import HomePage from './Pages/HomePage'
import SearchPage from './Pages/SearchPage'
import EditPage from './Pages/EditPage'

class Honest extends React.Component {
    render() {
        return (
            <div>
                <Router>
                    <div>
                        <div className="navbar navbar-default">
                            <div className="container-fluid">
                                <div className="navbar-header">
                                    <button type="button" className="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-navbar-collapse-1">
                                        <span className="sr-only">Toggle navigation</span>
                                        <span className="icon-bar"></span>
                                        <span className="icon-bar"></span>
                                        <span className="icon-bar"></span>
                                    </button>
                                    <a className="navbar-brand" href="#">Seppuku.Module.Honest</a>
                                </div>

                                <div className="collapse navbar-collapse" id="bs-navbar-collapse-1">
                                    <ul className="nav navbar-nav">
                                        <IndexLinkContainer to="/">
                                            <li><a href="#">Home</a></li>
                                        </IndexLinkContainer>
                                        <LinkContainer to="/search">
                                            <li><a href="#">Search</a></li>
                                        </LinkContainer>
                                        {/*<li className="dropdown">
                                    <a href="#" className="dropdown-toggle" data-toggle="dropdown" role="button" aria-expanded="false">Dropdown <span class="caret"></span></a>
                                    <ul className="dropdown-menu" role="menu">
                                        <li><a href="#">Action</a></li>
                                        <li><a href="#">Another action</a></li>
                                        <li><a href="#">Something else here</a></li>
                                        <li className="divider"></li>
                                        <li><a href="#">Separated link</a></li>
                                        <li className="divider"></li>
                                        <li><a href="#">One more separated link</a></li>
                                    </ul>
                                </li>*/}
                                    </ul>
                                    <ul className="nav navbar-nav navbar-right">
                                        <li><a href="https://netdex.cf">netdex.cf</a></li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <div style={{ position: 'relative' }}>
                            <Route render={({ location, history, match }) => {
                                return (
                                    <RouteTransition
                                        pathname={location.pathname}
                                        className="transition-wrapper"
                                        atEnter={{ opacity: 0 }}
                                        atLeave={{ opacity: 0 }}
                                        atActive={{ opacity: 1 }}

                                    >
                                        <Switch key={location.key} location={location}>
                                            <Route exact path="/" component={HomePage} />
                                            <Route path="/search" component={SearchPage} />
                                            <Route path="/edit" component={EditPage} />
                                        </Switch>
                                    </RouteTransition>
                                );
                            }} />
                        </div>
                    </div>
                </Router>
            </div>
        );
    }

}

export default Honest;
