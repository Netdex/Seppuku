# Seppuku.Module.Honest


## What is this?
`Honest` is a seppuku-backed web application that stores messages for people (typically brutally honest messages), and releases them upon deadman switch trigger.

This module would be used to leave information or messages to certain people after death/incapacitation.

## Authentication
To ensure only the correct party receives messages, `Honest` uses an authentication system to deliver messages.

### Supported Authentication Methods

#### Phone Number Verification
Phone call/text message verification via Twilio API.

#### Email Verification
Email verification via SMTP.