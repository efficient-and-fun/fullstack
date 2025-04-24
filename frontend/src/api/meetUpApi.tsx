import { MeetUp } from "../models/MeetUp";
import { MeetUpDetail } from "../models/MeetUpDetails";
import { Dayjs } from "dayjs";

function meetUpsApiCall(
  url: string,
  setEvents: React.Dispatch<React.SetStateAction<MeetUp[]>>,
  selectedDate: Dayjs
) {
  fetch(url)
    .then((res) => {
      if (!res.ok) {
        throw new Error(
          `Fehler meetUpsApiCall (Status ${res.status})`
        );
      }
      return res.json() as Promise<MeetUp[]>;
    })
    .then((data) => {
        const meetUps = data.map((meetUp) => ({
          ...meetUp,
          dateTimeFrom: meetUp.dateTimeFrom
            ? new Date(meetUp.dateTimeFrom)
            : null,
          dateTimeTo: meetUp.dateTimeTo ? new Date(meetUp.dateTimeTo) : null,
        }));
        setEvents(meetUps);
    })
    .catch((err) => {
        console.log(err);
    })
    .finally(() => {
    });
}

function meetUpApiCall(
  url: string,
  setEvent: React.Dispatch<React.SetStateAction<MeetUpDetail>>,
  id: number
) {
  fetch(`${url}${id}`)
    .then((res) => {
      if (!res.ok) {
        throw new Error(`Fehler meetUpApiCall (Status ${res.status})`);
      }
      return res.json() as Promise<MeetUpDetail>;
    })
    .then((data) => {
      const meetUp = {
        ...data,
        dateTimeFrom: data.dateTimeFrom ? new Date(data.dateTimeFrom) : null,
        dateTimeTo: data.dateTimeTo ? new Date(data.dateTimeTo) : null,
      };
      
      setEvent(meetUp);
    })
    .catch((err) => {
      console.log(err);
    })
    .finally(() => {});
}

export { meetUpsApiCall, meetUpApiCall }
