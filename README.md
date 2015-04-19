# Enabling Distributed Architectures with a Service Bus

## April 23, 2015

Traditional fat-client/desktop applications can scale locally by running multiple threads to handle long running, asynchronous, or secondary processes in the background.

In the web world, the request/response cycle is very short lived and you do not want to burden your web server with other chores beyond processing web requests. To handle this scenario, you need to break your application apart into multiple disconnected services that all work together.

One of the key enablers for breaking an application apart is a Service Bus, allowing messages to be passed between the services. The best part... the services don't need to know anything about each other; they only need to agree on the message formats.
