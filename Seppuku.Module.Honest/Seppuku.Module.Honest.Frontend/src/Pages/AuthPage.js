import React from 'react'
import { Form, Text } from 'react-form'

class AuthPage extends React.Component {

    authenticate(values) {
        fetch("/honest/api/login", {
            method: 'POST',
            body: values
        }).then(response => {
            return response.json();
        }).then(j => {
            
        }).catch(err => {

        });
    }

    render() {

        return (
            <div>
                <h1>Authentication Required</h1>
                <hr />
                <Form onSubmit={(values) => {
                    this.authenticate(values);
                }}>
                    {({ submitForm, setValue }) => {
                        return (
                            <form onSubmit={e => {
                                submitForm(e);
                                setValue('password', '', true);
                            }}>
                                <div className='form-group'>
                                    <label htmlFor='password'>Password</label>
                                    <Text className="form-control" field='password' placeholder="Password" type="password" />
                                </div>
                                <button type="submit" className='btn btn-primary'>Submit</button>
                            </form>
                        )
                    }}
                </Form>
            </div>
        );
    }
}

export default AuthPage;