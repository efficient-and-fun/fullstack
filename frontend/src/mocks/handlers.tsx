// src/mocks/handlers.ts
import { http, HttpResponse } from 'msw';
import {MeetUp } from '../models/MeetUp';
import { MeetUpDetail } from '../models/MeetUpDetails';
import eventsRaw from '../components/Home/DailyView/testEvents.json';

interface User {
  id: number;
  username: string;
}

function getEventDetailbyID(id:number) {
  console.log(eventsRaw);
         const event = eventsRaw.find(event => {
          return event.meetUpId == id;
         });
         
         
         return event;
}

export const handlers = [
  http.get('/api/meetup/1', () => {

    return HttpResponse.json(eventsRaw);
  }),
  http.get('/api/meetup/1/:id', (req) => {
    const { id } = req.params;
    const numericId = Number(id);
    return HttpResponse.json(getEventDetailbyID(numericId))
  }),
  http.post('/api/login', (req) => {
    let response = { token : "asdf"};
    return HttpResponse.json(response);
  }),
  http.get('/api/validate', async ({request}) => {
    return HttpResponse.json({message: 'Success'});
  })
];
