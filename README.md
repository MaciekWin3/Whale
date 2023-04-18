# Whale 🐳

## 1) Software Characteristics
Short Name: Whale

Full Name: Whale is a lightweight terminal user interface application that helps programmers and devops manage Docker containers locally or remotely.

Description: The Whale application aims to simplify Docker container management through an intuitive terminal-based interface. Users will be able to display, start, stop, create, and delete containers, as well as view their logs and information about ports and environment variables. 

## 2) Copyright
a. Authors: Maciej Winnik

b. License terms for the software developed by the group: The Whale application will be available under the open source license according to the **MIT License** terms.

The **MIT License** is a permissive software license that allows the distribution of the licensed software under the condition that the license is included in the distribution and in any modifications made to the software. The license also disclaims any warranty or liability for the software. In short, the MIT License is a flexible and liberal license that allows users to use, modify, and distribute the software for any purpose.

## 3) Requirements specification
ID: FUNC-REQ-01\
Name: Display container information
Description: The application should allow users to display information about containers, such as their status, image, and ID.\
Priority: 1 (needed)\
Category: Functional\

ID: FUNC-REQ-02\
Name: Start/Stop containers\
Description: The application should allow users to start and stop containers.\
Priority: 1 (needed)\
Category: Functional\

ID: FUNC-REQ-03\
Name: Create/Delete containers\
Description: The application should allow users to create and delete containers.\
Priority: 1 (needed)\
Category: Functional\

ID: FUNC-REQ-04\
Name: Display container logs\
Description: The application should allow users to view logs from containers.\
Priority: 2 (good to have)\
Category: Functional\

ID: FUNC-REQ-05\
Name: Display container port information\
Description: The application should allow users to view information about the ports used by containers.\
Priority: 2 (good to have)\
Category: Functional\

ID: FUNC-REQ-06\
Name: Docker Swarm Management\
Description: The application should provide features for managing Docker swarms, including creating and removing swarms, adding and removing nodes, and deploying and scaling services.\
Priority: 1\
Category: Functional\

ID: FUNC-REQ-07\
Name: Container volume management\
Description: The Whale application should allow users to manage container volumes, including creating, attaching, detaching, and deleting volumes.\
Priority: 1 (needed)\
Category: Functional\

ID: FUNC-REQ-08\
Name: Docker Compose support\
Description: The Whale application should support Docker Compose files, allowing users to easily create and manage multi-container Docker applications.\
Priority: 2 (good to have)\
Category: Functional\

ID: NON-FUNC-REQ-01\
Name: Cross-platform compatibility\
Description: The application should be compatible with multiple platforms, including Windows, macOS, and Linux.
Priority: 1 (needed)\
Category: Non-functional\

ID: NON-FUNC-REQ-02\
Name: Usability\
Description: The application should have an intuitive interface and be easy to use for both experienced and inexperienced users.
Priority: 1 (needed)\
Category: Non-functional\

ID: NON-FUNC-REQ-03\
Name: Performance\
Description: The application should have fast response times and should not use excessive system resources.
Priority: 1 (needed)\
Category: Non-functional\

ID: NON-FUNC-REQ-04\
Name: Extensibility\
Description: The application should be designed in a modular and extensible way to allow for future updates and new features.
Priority: 3 (optional)\
Category: Non-functional\

ID: NON-FUNC-REQ-05\
Name: Keyboard shortcuts\
Description: The Whale application should support keyboard shortcuts to help users perform common tasks more quickly and efficiently.\
Priority: 2 (good to have)\
Category: Non-functional\

ID: NON-FUNC-REQ-06\
Name: Theme customization\
Description: The Whale application should allow users to customize the application's color scheme and appearance.
Priority: 3 (optional)\
Category: Non-functional\

## 4) Architecture of the system/software:
###  Development architecture - technology stack:
* C# language
* .NET platform
* Terminal.Gui library
* Docker Engine
### Runtime architecture - technology stack:
* Docker
* Docker Swarn
* Termianal (Powershell/Linux)
* Git
* GitHub

The Whale application is based on the Terminal.Gui library and written in C# version 11. Additionally, we use the .NET platform. In order to ensure proper application functionality, a Terminal (such as PowerShell or Linux terminal) and Docker Engine are required. The application is designed to assist in managing Docker swarm.

## 5) Test cases
| Requirement ID  | Functionality                      | Test Objective                                   | Precondition                                                          | Action                                                     | Expected Result                                                                                                      |
|-----------------|------------------------------------|--------------------------------------------------|-----------------------------------------------------------------------|------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------|
| FUNC-REQ-01     | Display container information      | Display container details                        | User is logged in and has containers running                          | Click on container details button                          | Information about the container, such as its status, image, and ID, is displayed                                     |
| FUNC-REQ-02     | Start/Stop containers              | Start and stop containers                        | User is logged in and has containers running                          | Click on start/stop button                                 | Container is started/stopped successfully                                                                            |
| FUNC-REQ-03     | Create/Delete containers           | Create and delete containers                     | User is logged in and has necessary permissions                       | Click on create/delete button                              | Container is created/deleted successfully                                                                            |
| FUNC-REQ-04     | Display container logs             | View container logs                              | User is logged in and has containers running                          | Click on container logs button                             | Logs from the container are displayed                                                                                |
| FUNC-REQ-05     | Display container port information | View container port information                  | User is logged in and has containers running                          | Click on container port button                             | Information about the ports used by the container is displayed                                                       |
| FUNC-REQ-06     | Docker Swarm Management            | Manage Docker swarms                             | User is logged in and has necessary permissions                       | Click on swarm management button                           | Swarm is created/removed, nodes are added/removed, services are deployed/scaled successfully                         |
| FUNC-REQ-07     | Container volume management        | Manage container volumes                         | User is logged in and has necessary permissions                       | Click on volume management button                          | Volume is created, attached/detached, and deleted successfully                                                       |
| FUNC-REQ-08     | Docker Compose support             | Support for Docker Compose files                 | User has necessary permissions and a Docker Compose file is available | Upload Docker Compose file                                 | Multi-container Docker application is created and managed successfully                                               |
| NON-FUNC-REQ-01 | Cross-platform compatibility       | Compatibility with multiple platforms            | User has access to multiple platforms                                 | Install and launch application on different platforms      | Application launches and functions correctly on all platforms                                                        |
| NON-FUNC-REQ-02 | Usability                          | Intuitive interface and ease of use              | User is new to the application                                        | Navigate through the application and perform basic tasks   | User can navigate through the application and perform basic tasks without difficulty                                 |
| NON-FUNC-REQ-03 | Performance                        | Fast response times and efficient resource usage | User has multiple containers running                                  | Start and stop containers, view container details and logs | Containers start/stop quickly, details and logs are displayed quickly, and application uses minimal system resources |
| NON-FUNC-REQ-04 | Extensibility                      | Modular and extensible design                    | Application is in development                                         | Implement new features and updates                         | New features and updates are easily implemented without causing issues or breaking existing functionality            |
| NON-FUNC-REQ-05 | Keyboard shortcuts                 | Keyboard shortcuts for common tasks              | User has access to a keyboard                                         | Use keyboard shortcuts for basic tasks                     | Basic tasks can be performed more quickly and efficiently using keyboard shortcuts                                   |
| NON-FUNC-REQ-06 | Theme customization                | Customizable color scheme and appearance         | User wants to customize the application's appearance                  | Access theme customization options                         | Application's color scheme and appearance can be customized to the user's preferences                                |

