# RaceDay API Endpoint Plan

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | /api/auth/register | Creates a participant account. | None (public) | firstName, lastName, email, password | 201 Created; 400 Bad Request; 409 Conflict |
| POST | /api/auth/login | Authenticates a user. | None (public) | email, password | 200 OK; 401 Unauthorized |
| GET | /api/users/me | Gets the logged-in user's profile. | Any logged in | None | 200 OK; 401 Unauthorized |
| PUT | /api/users/me | Updates the logged-in user's profile. | Any logged in | profile fields | 200 OK; 400 Bad Request; 401 Unauthorized |
| GET | /api/events | Lists upcoming events. | None (public) | None | 200 OK |
| GET | /api/events/{id} | Gets one event and its categories. | None (public) | None | 200 OK; 404 Not Found |
| POST | /api/events | Creates an event. | Organiser | event fields | 201 Created; 400 Bad Request; 403 Forbidden |
| PUT | /api/events/{id} | Updates an event. | Organiser | event fields | 200 OK; 403 Forbidden; 404 Not Found |
| DELETE | /api/events/{id} | Deletes an event. | Organiser | None | 204 No Content; 403 Forbidden; 404 Not Found |
| GET | /api/events/{eventId}/categories | Lists event categories. | None (public) | None | 200 OK; 404 Not Found |
| POST | /api/events/{eventId}/categories | Creates a category. | Organiser | categoryName, ageGroup, entryFee, maxParticipants | 201 Created; 400 Bad Request; 403 Forbidden |
| PUT | /api/categories/{id} | Updates a category. | Organiser | category fields | 200 OK; 403 Forbidden; 404 Not Found |
| DELETE | /api/categories/{id} | Deletes a category. | Organiser | None | 204 No Content; 403 Forbidden; 404 Not Found |
| POST | /api/enrolments | Enrols the logged-in participant. | Participant | categoryId | 201 Created; 401 Unauthorized; 409 Conflict |
| GET | /api/enrolments/me | Gets the participant's enrolments. | Participant | None | 200 OK; 401 Unauthorized |
| GET | /api/events/{eventId}/enrolments | Views all enrolments for an event. | Organiser | None | 200 OK; 403 Forbidden; 404 Not Found |
| DELETE | /api/enrolments/{id} | Cancels an enrolment. | Participant | None | 204 No Content; 403 Forbidden; 404 Not Found |
| POST | /api/results | Captures a participant result. | Organiser | enrolmentId, finishPosition, finishTime, resultStatus | 201 Created; 400 Bad Request; 409 Conflict |
| GET | /api/results/me | Gets personal results. | Participant | None | 200 OK; 401 Unauthorized |
| GET | /api/events/{eventId}/results | Gets results for an event. | Organiser | None | 200 OK; 403 Forbidden; 404 Not Found |
| GET | /api/results/{id} | Gets one result. | Any logged in | None | 200 OK; 404 Not Found |
