import React from 'react';
import ReactDOMServer from 'react-dom/server';

import { createServerRenderer } from 'aspnet-prerendering';
import * as Templates from './templates'

const EmptyComponent = (props) => (<div></div>);

const getTemplate = (props) => {
	return Templates[props.templateName]["default"];
};

const getRootComponent = (props) => {
    return React.createElement(getTemplate(props.data), props.data);
};

const getHtml = (params) => {
    const component = getRootComponent(params);
    const html = ReactDOMServer.renderToString(component).replace(/^\s|\s$/g, '');

    return { html };
}

export default createServerRenderer(params => new Promise(resolve => resolve(getHtml(params))));
