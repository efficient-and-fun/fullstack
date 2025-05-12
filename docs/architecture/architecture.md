# Architecture
## 1. Introduction and Goals
Our event planning app aims to provide an intuitive platform for both private and public events.
Created events should be bookable for individual users. Additionally, users can create and join
friend groups. When a large event, such as the Greenfield Festival, is approaching, users can
plan the event within these friend groups.

Many event planning tools lack seamless coordination between individual and group event
management. Users often struggle with disorganized planning, scattered communication, and
difficulty in keeping track of event details. Our app addresses these challenges by offering:
### 1.1 Quality Goals
| Goal                             | Description                                                                                       |
|----------------------------------|---------------------------------------------------------------------------------------------------|
| **Centralized event management** | Provides a unified platform to manage both private and public events efficiently.                 |
| **Seamless group coordination**  | Allows users to create and join friend groups for better event planning and participation.        |
| **Integrated booking system**    | Ensures easy access to events by providing a built-in booking and reservation feature.            |
| **Comprehensive event details**  | Displays key event information, including date, location (with map), required items, and pricing. |
| **Calendar synchronization**     | Enables integration with external calendars like Google Calendar for better scheduling.           |
### 1.2 Stakeholder
| Who?                             | Expectation?                                                                             |
|----------------------------------|------------------------------------------------------------------------------------------|
| **Individual Users**             | Need an intuitive interface for event discovery, booking, and participation.             |
| **Event Organizers**             | Require efficient tools for event creation, attendee management, and logistics planning. |
| **Friend Groups**                | Benefit from collaborative planning features for shared events.                          |
| **Business& Public Event Hosts** | Need visibility to attract attendees and manage large-scale event logistics.             | 
                                                                                                           

## 2. Constraints
## 3. Context and Scope
## 4. Solution Strategy
## 5. Building Block View
## 6. Runtime View
## 7. Deployment View
As this is the scope of a school project we were limited to the available hardware environemnt our school provided. Therefore we choose to use rancher paired with argoCD to host our enivronment. Our building pipline uses Github Actions. In the following sectino a typical CI-CD pipeline run is described. It triggers when something is pushed into the dev branch on the fullstack repository.



### GitHub Actions
1. We use two different building pipelines for frontend and backend. They get triggered when there are changes in the folder backend or frontend in the dev branch. The action runs all the test.
2. If all tests pass a Docker image gets built and pushed to the GHCR of the organization. It is tagged with the current commit hash. 
3. At last the workflow updates the operation repository, using a dipsatch event. It changes the image tag to the newly built one.

### Argo CD
4. ArgoCD watches the dev and main branch for changes. It realizes, that there were changes in the operations repository. It pulls the updated manifest and compares it to the current one.
5. If there are changes detected. ArgoCD tells rancher what ressources to update.
7. If there are problems with the newly applied ressources ArgoCD tells Rancher to fall back to a older working version.

### Rancher
6. Rancher pulls the new image and tries to set it up.

On the Rancher Kluster there are two running namespaces staging and production.
Staging is used for testing reasons to run the application in the real environment. It mirrows the state of the dev branch in the operations repository.
Main is the running application. It mirrows the state of the main branch in the operations repository. To release a new version we need to create a pull request from dev to main branch in the operations repository.
    
## 8. Cross-cutting Concepts
How is the deployment process?
## 9. Architecture Decisions

### HTTP Methoden:
- POST: Creation / Insert
- PUT: Update

## 10. Quality Requirements
## 11. Risks and Technical Debt
## 12. Glossary