# Introduction

This service provides email templating through React.js, with automatic sending and logging of emails.

You create a react template:

```
const HelloWorld = (props) => (
	<div>
		<h1>Hello world!</h1>
		<p>Payload: {JSON.stringify(props)}</p>
	</div>
);

export default HelloWorld;
```

You add the template to the list of templates in `EmailService\wwwroot\javascripts\email.js`.

```
...
import HelloWorld from './custom/templates/helloworld';

const emailTemplates = {
	"helloworld": HelloWorld
};
...
```

You configure sender name and email address, as well as a mapping for what email subject each template should have in `EmailService\Properties\emailProperties.json`:

```
{	
	"fromAddress": "a@gmail.com",
	"fromName": "A",
	
	"templates": [
		{
			"name": "helloworld",
			"subject": "HelloWorld example email!"
		}
	]
}
```

You configure what email service to use, and what logging method to use in `EmailService\appsettings.json`, see Configuration section below.

Then you can start sending emails through the service API:

```
# /api/email/send
{
	"to": ["a@gmail.com"],
	"template": "helloworld",
	"content": {
		"some": "react content"
	},
	"personalContent": {
		"HelloTo": "ourrobin"
	}
}
```

## content vs personalContent

To comply with GDPR the content that will be consumed by the React email template is divided into two parts: content and personalContent. When the email is generated, both of the objects content and personalContent are merged and sent to the React template in the props attributes. The difference is that when the logs are created, the content will be stored as is in plain text, whereas the personalContent will be obfuscated, and stored like this:

```
personalContent
"{ \"HelloTo\": \"str(8)\" }"
```

So you are adviced to put all personal user data in the personalContent param, and the non-user-related content in the content param.


# Configuration

The following needs to be added to "appSettings.json":

```
"EmailService": "", 		//"sendgrid" or other service that is supported
"EmailServiceUrl": "",		//url to the email service API
"EmailServiceApiKey": "",	//your api key

"LoggingType": "",							//"database", "api" or "none"
"LoggingApiUrl": "",						//if LoggingType is "api", you should provide the API endpoint to which logs should go
"LoggingDatabaseConnectionString": "",		//if LoggingType is "database", you should provide the database connection string to which logs should go
```


# Endpoint summaries:

## GET api/email

Healthcheck for the service. Returns 200 OK if operational.

## GET api/email/{id}

Returns information about an email that has been sent.
This requires the logging method to be "database", otherwise this endpoint will not return a 200 OK result.

## POST api/email

Preview an email, without actually sending it.

## POST api/email/send

Send an email.