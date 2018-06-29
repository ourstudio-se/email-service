
import React from 'react';
import ReactDOMServer from 'react-dom/server';

import { createServerRenderer } from 'aspnet-prerendering';

const Email = (props) => (
	<h1>Hello world!</h1>
);

const getRootComponent = (params) => {
    return React.createElement(Email, {});
};

const getHtml = (params) => {
    const component = getRootComponent(params);
    const html = ReactDOMServer.renderToString(component).replace(/^\s|\s$/g, '');

    return { html };
}

export default createServerRenderer(params => new Promise(resolve => resolve(getHtml(params))));
