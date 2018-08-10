# CloverQ
---
CloverQ is an ACD (Automatic Call Distributor) written in C#. The main features are that can queue calls form multiple Asterisk servers, and can handle endpoints on different Asterisk server. The idea behind that is offload the registration and outbound traffic from the Astersiks working as queue servers. At the moment only works over chan_sip.

## Features
---
* Allows to spread the queues on multiple Asterisk servers
* Maintains the global order of a queued call
* Connect rtp of a queued call directly to the agent's sip phone
* Allows you to define queues and agents from a configuration file
* Manage agents independently of Asterisk