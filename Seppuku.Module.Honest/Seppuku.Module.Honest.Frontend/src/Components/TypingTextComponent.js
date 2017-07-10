import React from 'react'

class TypingTextComponent extends React.Component {

    constructor(props) {
        super(props);
        this.state = { delay: this.props.delay ? this.props.delay : 500 };
    }

    onTextChanged(text) {
        this.props.onChange(text);
    }

    handleChange(e) {
        e.preventDefault();
        
        clearTimeout(this.state.timeoutId);

        let timeoutId = setTimeout(() => {
            this.onTextChanged(this.state.value);
        }, this.state.delay);

        this.setState({ timeoutId: timeoutId, value: e.target.value });
    }

    render() {
        return (
            <input type="text" className={this.props.className} onChange={e => this.handleChange(e)} />
        );
    }
}

export default TypingTextComponent;