
import React from 'react';
import ReactDOM from 'react-dom';

const Email = (props) => (
	<h1>Hello world!</h1>
);

window.addEventListener('load', function () {
	ReactDOM.render(<Email />, document.querySelector('#application-node'));
}, true);

