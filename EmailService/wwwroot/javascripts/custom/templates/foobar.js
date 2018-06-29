import React from 'react';

const Foobar = (props) => (
	<div>
		<h1>Hello world!</h1>
		<p>Payload: {JSON.stringify(props)}</p>
	</div>
);

export default Foobar;