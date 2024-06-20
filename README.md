# Mikieta API

## Live
www.ptrwilk.pl

## Setup Instructions

Clone the repository. The main branch is `dev`.

To run the API properly, it is recommended to set up the `appsettings.json` file. For convenience, make a copy of `appsettings.json` and name it `appsettings.Development.json`.

### Stripe Configuration

```json
"Stripe": {
  "SecretKey": "",
  "PublishableKey": ""
}
```

To enable payment processing with Stripe, you need to fill in the above two keys. To obtain these keys, you must register on the [stripe.com].

### SMTP Client Configuration

```json
"SmtpClient": {
  "Host": "",
  "Port": 0,
  "Email": "",
  "Password": ""
}
```

To enable email sending functionality, you need to fill in the appropriate fields above.
An examplary case for gmail can be found here: [IMAP, POP, and SMTP]

### Google API Key

```
"GoogleApiKey": ""
```
To use the distance estimation feature, you need to provide a key generated from the [Google API].

### JWT Configuration

```json
"Jwt": {
  "Key": ""
}
```

To ensure proper authentication with [Panel Restauratora], you need to provide a randomly generated key. You can use a tool like [GUIDGenerator] to generate a key.






[stripe.com]: <https://stripe.com/en-pl>
[IMAP, POP, and SMTP]: <https://developers.google.com/gmail/imap/imap-smtp>
[Google API]: <https://console.cloud.google.com/apis/credentials>
[Panel Restauratora]: <http://google.pl>
[GUIDGenerator]: <https://www.guidgenerator.com/>