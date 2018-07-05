# Introduction

This service provides email templating through React.js, with automatic sending and logging of emails.

Basically it allows you to write email templates in JavaScript with React.js:

```
const HelloWorld = (props) => (
	<div>
		<h1>Hello world!</h1>
		<p>Payload: {JSON.stringify(props)}</p>
	</div>
);

export default HelloWorld;
```

Then with a little bit of configuration, you can start sending emails through the service API:

```
# POST /api/email/send
{
	"to": ["b@gmail.com"],
	"template": "HelloWorld",
	"content": {
		"some": "react content"
	},
	"personalContent": {
		"HelloTo": "ourrobin"
	}
}
```

If configured, the app will then log all your email requests either to a database or an external API.

## content vs personalContent

To comply with GDPR the content that will be consumed by the React email template is divided into two parts: content and personalContent. When the email is generated, both of the objects content and personalContent are merged and sent to the React template in the props attributes. The difference is that when the logs are created, the content will be stored as is in plain text, whereas the personalContent will be obfuscated, and stored like this:

```
personalContent
"{ \"HelloTo\": \"str(8)\" }"
```

So you are adviced to put all personal user data in the personalContent param, and the non-user-related content in the content param.


# Installation and Configuration

Make sure you have the following installed:

* NodeJs
* Yarn
* Dotnet SDK and runtime v. 2.1

Note: The app is divided into two parts; the service itself and your own app. Your own app contains all of the email templates and configurations used to run the email service.

With that in mind, you should then:

1. Clone this github repository
2. Copy the content of the folder `EmailService/app_example` to your own directory wherever you prefer. This is the content of your own app, and you are adviced to __not__ store the content inside of this repository, but instead make your own version control repository and keep the content there. This is to be able to version control your own email configurations and email templates, without having to use this repository.
3. Navigate to your own application directory
4. Edit the file `configurations/buildConfigurations.json` and enter the targetDirectory. The target directory should be an absolute path to wherever this repository (the email service repository) is located, and to the `app_content` directory inside of this repository. So for instance: `c:\\development\\email-service\\EmailService\\app_content`. This will allow you to build and run webpack from your own app directory, but the build output will be put inside of this repository's folder `app_content` so that the service can consume the content when ran.
5. Edit the file `configurations/serviceConfiguration.json`. See separate section below.
5. Create your own React templates...
5. Export your React templates inside of the file `javascripts\templates\index.js`
5. Edit the file `configurations/emailConfiguration.json`. See separate section below
5. Run `yarn install` to install the JavaScript dependencies
5. Run `yarn watch` to create the JavaScript build
5. Navigate to the service repository directory
5. Double check that there is now three files inside of the `EmailService/app_content` directory: `default.bundle.js`, `emailConfiguration.json` and `serviceConfiguration.json`
5. Build the dotnet service with: `dotnet build EmailService/EmailService.csproj -c Release`
5. Run the dotnet service with: `dotnet EmailService\bin\Release\netcoreapp2.1\EmailService.dll`

## Configure serviceConfiguration.json

The configuration should have the following format:

```
{
	"EmailService": "",
	"EmailServiceUrl": "",
	"EmailServiceApiKey": "",

	"LoggingType": "",
	"LoggingApiUrl": "",
	"LoggingDatabaseConnectionString": ""
}
```

With the following values:

Key | Values
--- | ------
`EmailService` | One of the supported external email services that should be used to send the emails, which currently are: `sendgrid`
`EmailServiceUrl` | A URL to the external email services endpoint for sending an email. The HTTP method used is POST.
`EmailServiceApiKey` | An api key to the external email service.
`LoggingType` | Enter `database` if you want to log to a database, `api` if you want to log to an external API or `none` if you do not want logging of emails sent.
`LoggingApiUrl` | If you chose logging type `api`, you should enter the URL of the api to send logs to here. The HTTP method used is POST.
`LoggingDatabaseConnectionString` | If you chose logging type `database`, you should enter a connection string for your SQL database here. See [this page](https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.connectionstring(v=vs.110).aspx) for information about the format.


## Configure emailConfiguration.json

The configuration should have the following format:

```
{	
	"FromAddress": "",
	"FromName": "",
	
	"Templates": [
		{
			"Name": "",
			"Subject": ""
		}
	]
}
```

With the following values:

Key | Values
--- | ------
FromAddress | The email address that the email will appear to be from. This is the email address that the user receiving the email will see in their email client as sender.
FromName | The name of the email sender. Will also be visible by the user receiving the email in their email client.
Templates | An array of "Templates", which are JSON object mapping a template name to a subject. This is to make sure that each email template have their own email subject, which is the subject displayed to the user receiving the email in their email client. The name should match a React template name, and the subject can be anything.


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