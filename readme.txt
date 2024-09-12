The provided project is a Windows Forms semi completed TCP Sockets Chat project. 

How To Run the Project
========================
To run it you will need to run at least 2 instances of the project exe.
Host Server:
If you open the project in Visual Studio and run it, choose to Host Server. 
Host Client(s):
In the project folder, navigate to Windows Forms core chat\bin\Debug\netcoreapp3.1 folder and run Windows Forms core chat.exe and click Join Server. 

Clients should be able to type messages to the server and the server can broadcast those messages to all clients.

The Project
============
In Visual Studio if you open Form1.cs it should open the Windows Forms designer. If you right click textboxes or buttons you can see that they have a name property which is how we can access them via code. Take note of the main ui elements names here.

If you right click Form1.cs in the solution explorer you can 'View Code'. Explore the code and try to understand it.

The server creates an asynchronous task(thread) to keep listening for joining clients. If a client joins, a new thread is created to start listening for incoming messages from that client. So in total, the server has the main thread the UI runs on, the listening to joining client threads and as many extra threads running for joined clients sending messages in.

You will need to work through the code to work out where and how to add to it.
==============================================================================================
**********************************************************************************************
The code has been altered and many features has been added to it.
The features are:
1. Clients are able to allocate themselves a username and are able to change it whenever they want given that the selected username is unique in the server.
2. The client and server messages are followed by the sender of the message.
3. The server can designate a client as a moderator who can kick other members from the servers.
4. The server can also remove a moderator form its role.
==============================================================================================
Client Commands
|Command	|Syntax				|Function			
-----------------------------------------------------------------------------------------
|!command	|!command       		|Displays available commands 
|!who		|!who				|Displays list of memebers of the server
|!whisper	|!whisper [username][message]	|Send private message to a member of the server
|!username	|!username [new_username]	|Change username		
|!events	|!events			|Display list of available events
|!exit		|!exit				|Leave the server
-----------------------------------------------------------------------------------------------
===============================================================================================
Moderator Commands
|Command		|Syntax			|Function
-------------------------------------------------------------------------------------------
|!kick			|!kick [username]	|Kick the user from the server
-------------------------------------------------------------------------------------------
==============================================================================================
Server Commands
|Command		|Syntax				|Function
-------------------------------------------------------------------------------------------
|!mod			|!mod [username]		|If user is not a mod, designate them as mod and if the user is a mod, then remove them as a mod
|!event_add		|!event_add [event]		|Add event to the server
|!event_remove		|!event_remove [event index]	|Remove event from the server based on its index starting from 0
------------------------------------------------------------------------------------------------------------------------------------------
========================================================================================================================================
************************************************************************************************
***********************************************************************************************

------------------------------------------------------------------------------------------------------------------
Integrated database with the remote server by storing the members of the server in a database.
The database stores the id, username, password, wins, losses and draws of each member of the server.
If new member want to join the server, new username and password can be registered which is then stored in the database 
------------------------------------------------------------------------------------------------------------
Implemented the tic tac toe game between two member of the server.
!play initiates the game.
If there is no one is the queue then the member gets added to the queue.
If there are two players in the queue then the game begins with the first being player one and the second being player 2.
When the game ends, the result is displayed, the queue is emptied out, the database is updated and new members can initiate the game again.
----------------------------------------------------------------------------------------------------------------------------------------
By using !Stats the member of the chat are able to view the scores of all the members of the chat.
---------------------------------------------------------------------------------------------------------------------------
========================================================================================================================
