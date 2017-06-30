import React from 'react'
import {
    BrowserRouter as Router,
    Route,
    Link
} from 'react-router-dom'
import SearchComponent from './SearchComponent'

import './Honest.css';

/*

Component Summary
=================

1. Search Component
2. Authentication Component
3. Data View Component


*/



class Honest extends React.Component {

    render() {
        return (
            <Router>
                <Route path="/" component={SearchComponent} />
            </Router>
        );
    }

}

export default Honest;
