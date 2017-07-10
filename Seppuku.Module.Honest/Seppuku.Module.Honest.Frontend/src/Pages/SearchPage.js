import React from 'react'
import { Form, Text } from 'react-form'
import TypingText from '../Components/TypingTextComponent'

class SearchPage extends React.Component {

    render() {
        return (
            <div style={{ position: 'absolute', width: '100%' }}>
                <div className="panel panel-default" >
                    <div className="panel-body" >
                        <p>Enter your name below to see if you are present in the database.</p>
                        <hr/>
                        <div className='form-group'>
                            <label htmlFor="identity">Search Query</label>
                            <TypingText className="form-control" onChange={t => { console.log(t) }} />
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

export default SearchPage; 