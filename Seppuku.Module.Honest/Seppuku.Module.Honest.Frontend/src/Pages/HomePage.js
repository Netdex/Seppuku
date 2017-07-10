import React from 'react'
import {Link} from 'react-router-dom'

class HomePage extends React.Component {

    render() {
        return (
            <div style={{ position: 'absolute', width: '100%' }}>
                <div className="panel panel-default" >
                    <div className="panel-body" >
                        <h1><code>Honest.Pages.HomePage</code></h1>
                        <h2>FAQ</h2>
                        <h4>What is Honest?</h4>
                        <p>This application is a module for <a href="https://github.com/Netdex/Seppuku">Seppuku</a>, a digital deadman's switch acting 
                            as an intangible executor.</p>
                        <p><code>Honest</code> allows a curator to stores messages for people, only publishing them after the curator is dead.
                            In order to see their private message from the curator, they must verify their identity through identities determined 
                            manually by the curator.</p>
                        <h4>Why?</h4>
                        <p>I thought being brutally honest to people might be fun, but only after I'll never see them again. 
                            Having such an application as a Seppuku extension module allows for this behavior.
                        </p>
                        <h4>I want my message!</h4>
                        <p>You can click on the "Search" tab above or <Link to="/search">click here</Link>, to see if you are present in the database. 
                            You can search for yourself with your name, then verify your identity with email, or text.</p>
                    </div>
                </div>
            </div>
        );
    }
}

export default HomePage; 