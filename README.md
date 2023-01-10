# Task-Tracker
### This is one of countless implementations of task-managers out there. 
###
The application is conteinerized. To run it locally with docker clone the repository and run the docker-compose.  
The seeding class will work automatically.  
Applicatin uses MS SQL database via Entity Framework.<br>
If you want to familiarize with the deployed version plesase feel free to follow the [link.](https://taskt-tracker-akvelon.herokuapp.com/swagger/index.html)<br>
Unlike the conteinerized app the deployed version is connected to the database host on freeasphosting.com<br>
###
This app is built using the traditionsl three-layer architecture. Using the repository pattern all the communication with the database is removed to corresponding repositories. Also validation services are used to insure the validity of input information.
