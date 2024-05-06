<h1>Learn RabbitMQ</h1>

<p>Main goal of this repo is to understand what is the RabbitMQ protocol and how to use it in the microservices architecture as well as how it implements on the backend.</p>

<div>
  <p align="center">
    <img src="https://miro.medium.com/v2/resize:fit:1400/format:webp/1*W-YLHnMseUTuvs5zb7tDhw.png" width="450" height="auto"/>  
  </p>
</div>

<h2>Microservices Communication Types Request-Driven or Event-Driven Architecture</h2>

<p>Microservices communications can be divide by 3<p>

<ul>
  <li>Request-Driven Architecture</li>
  <li>Event-Driven Architecture</li>
  <li>Hybrid Architecture</li>
</ul>

<h3>Request-Driven Architecture</h3>

<p>In the request-response based approach, services communicate using HTTP or RPC. In most cases, this is done using REST HTTP calls. But we have also developed that gRPC communication with Basket and Discount microservices. That communication was Request-Driven Architecture.</p>

<table>
 <tr>
   <th>Benefits</th>
   <th>Tradeoffs</th>
 </tr>
 <tr>
   <td>There is a clear control of the flow, looking at the code of the orchestrator, we can determine the sequence of the actions.</td>
   <td>If one of the dependent services is down, there is a high chance to exclude calls to the other services.</td>
 </tr>
</table>

<h3>Event-Driven Architecture</h3>

<p>This communication type microservices dont call each other, instead of that they created events and consume events from message broker systems in an async way.</p>

<table>
 <tr>
   <th>Benefits</th>
   <th>Tradeoffs</th>
 </tr>
 <tr>
   <td>The producer service of the events does not know about its consumer services. On the other hand, the consumers also do not necessarily know about the producer. As a result, services can deploy and maintain independently. This is a key requirement to build loosely coupled microservices.</td>
   <td>There is no clear central place defining the whole flow.</td>
 </tr>
</table>

<h3>Hybrid Architecture</h3>
<p>So it means that depending on your custom scenario, you can pick one of this type of communication and perform it.</p>

<h2>RabbitMQ</h2>
<p>RabbitMQ is a message queuing system. Similar ones can be listed as Apache Kafka, Msmq, Microsoft Azure Service Bus, Kestrel, ActiveMQ. Its purpose is to transmit a message received from any source to another source as soon as it is their turn. In other words, all transactions can be listed in a queue until the source to be transmitted gets up. RabbitMQ’s support for multiple operating systems and open source code is one of the most preferred reasons.</p>
<div>
  <p align="center">
    <img src="https://miro.medium.com/v2/resize:fit:720/format:webp/0*y5Ea5t4r33solXyN.png" width="450" height="auto"/>  
  </p>
</div>

<h3>Main Logic of RabbitMQ</h3>
<p><strong>Producer: </strong>The source of the message is the application.</p>
<p><strong>Queue: </strong>Where messages are stored. The sent messages are put in a queue before they are received. All incoming messages are stored in Queue, that is memory.</p>
<div>
  <p align="center">
    <img src="https://miro.medium.com/v2/resize:fit:720/format:webp/1*QnXMINGnhTjamBjGNpK2fQ.png" width="450" height="auto"/>  
  </p>
</div>
<p><strong>Consumer: </strong>It is the server that meets the sent message. It is the application that will receive and process the message on the queue.</p>
<p><strong>Message: </strong>The data we send on the queue.</p>
<p><strong>Exchange: </strong>It is the structure that decides which queues to send the messages. It makes the decision according to routing keys.</p>
<p><strong>Binding: </strong>The link between exchance and queue.</p>
<p><strong>FIFO: </strong>he order of processing of outgoing messages in RabbitMQ is first in first out.</p>
<div>
  <p align="center">
    <img src="https://miro.medium.com/v2/resize:fit:720/format:webp/0*bu9jtPMiDAaVblqa.png" width="450" height="auto"/>  
  </p>
</div>
<p>The Producer project sends a message to be queued. The message is received by the Exchange interface and redirects to one or more queues according to various rules.</p>

<h3>Queue Properties</h3>
<p><strong>Name: </strong>The name of the queue we have defined.</p>
<p><strong>Durable: </strong>Determines the lifetime of the queue. If we want persistence, we have to set it true. We use it in-memory in the project. In this case, the queue will be deleted when the broker is restart.</p>
<p><strong>Exclusive: </strong>Contains information whether the queue will be used with other connections.</p>
<p><strong>AutoDelete: </strong>Contains information about deletion of the queue with the data sent to the queue passes to the consumer side.</p>

<h3>RabbitMQ Exchange Types</h3>
<p>RabbitMQ is based on a messaging system like below.</p>
<div>
  <p align="center">
    <img src="https://miro.medium.com/v2/resize:fit:720/format:webp/0*LOP7gRWjKOSKOmLU.gif" width="450" height="auto"/>  
  </p>
</div>
<p><strong>Direct Exchange: </strong>The use of a single queue is being addressed. A routing key is determined according to the things to be done and accordingly, the most appropriate queue is reached with the relevant direct exchange.</p>
<p><strong>Topic Exchange: </strong>In Topic Exchanges, messages are sent to different queues according to their subject. The incoming message is classified and sent to the related queue. A route is used to send the message to one or more queues. It is a variation of the Publish / Subscribe pattern. If the problem concerns several consumers, Topic Exchange should be used to determine what kind of message they want to receive.</p>
<div>
  <p align="center">
    <img src="https://miro.medium.com/v2/resize:fit:640/format:webp/0*n6_HciUvaF3uSQrW.gif" width="450" height="auto"/>  
  </p>
</div>
<p><strong>Fanout Exchange: </strong>It is used in situations where the message should be sent to more than one queue. It is especially applied in Broadcasting systems. It is mainly used for games for global announcements.</p>
<div>
  <p align="center">
    <img src="https://miro.medium.com/v2/resize:fit:640/format:webp/0*65Sogvrnx-Rk6pLF.gif" width="450" height="auto"/>  
  </p>
</div>
<p><strong>Headers Exchange: </strong>Here you are guided by the features added to the header of the message. Routing-Key used in other models is not used. Transmits to the correct queue with a few features and descriptions in message headers. The attributes on the header and the attributes on the queue must match each other’s values.</p>
