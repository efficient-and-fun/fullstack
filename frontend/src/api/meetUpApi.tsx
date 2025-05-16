import { MeetUp } from "../models/MeetUp";
import { MeetUpDetail, NewMeetUpDetail } from "../models/MeetUpDetails";
import { Dayjs } from "dayjs";

function meetUpsApiCall(
  baseURL: string,
  setEvents: React.Dispatch<React.SetStateAction<MeetUp[]>>,
  selectedDate: Dayjs
) {
  const dateString = selectedDate.format("YYYY-MM-DD");
  const url = baseURL + "?currentDate=" + dateString;
  fetch(url)
    .then(async (res) => {
      if (res.status === 404) {
        return [];
      }
      if (!res.ok) {
        throw new Error(`Fehler meetUpsApiCall (Status ${res.status})`);
      }
      return res.json() as Promise<MeetUp[]>;
    })
    .then((data) => {
      const meetUps = data.map((meetUp) => ({
        ...meetUp,
        dateTimeFrom: meetUp.dateTimeFrom
          ? new Date(meetUp.dateTimeFrom)
          : null,
        dateTimeTo: meetUp.dateTimeTo
          ? new Date(meetUp.dateTimeTo)
          : null,
      }));
      setEvents(meetUps);
    })
    .catch((err) => {
      console.error("Unexpected error in meetUpsApiCall:", err);
      setEvents([]);
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
    .finally(() => { });
}

function updateMeetUpApiCall(
  meetUp: MeetUpDetail,
  onSuccess: () => void,
  onError?: (error: unknown) => void
) {
  const url = `/api/meetup/${meetUp.meetUpId}`;

  fetch(url, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      ...meetUp,
      dateTimeFrom: meetUp.dateTimeFrom
        ? new Date(meetUp.dateTimeFrom).toISOString()
        : null,
      dateTimeTo: meetUp.dateTimeTo
        ? new Date(meetUp.dateTimeTo).toISOString()
        : null,
    }),
  })
    .then(async (res) => {
      if (!res.ok) {
        throw new Error(`Fehler updateMeetUpApiCall (Status ${res.status})`);
      }

      const contentLength = res.headers.get("Content-Length");
      if (contentLength && parseInt(contentLength) > 0) {
        await res.json();
      }

      onSuccess();
    })
    .catch((err) => {
      console.error(err);
      if (onError) onError(err);
    });
}

function createMeetUpApiCall(
  meetUp: NewMeetUpDetail,
  onSuccess: () => void,
  onError?: (error: unknown) => void
) {
  const url = `/api/meetups`;

  fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      ...meetUp,
      dateTimeFrom: meetUp.dateTimeFrom
        ? new Date(meetUp.dateTimeFrom).toISOString()
        : null,
      dateTimeTo: meetUp.dateTimeTo
        ? new Date(meetUp.dateTimeTo).toISOString()
        : null,
    }),
  })
    .then(async (res) => {
      if (!res.ok) {
        throw new Error(`Fehler createMeetUpApiCall (Status ${res.status})`);
      }

      const contentLength = res.headers.get("Content-Length");
      if (contentLength && parseInt(contentLength) > 0) {
        await res.json(); // handle returned data if needed
      }

      onSuccess();
    })
    .catch((err) => {
      console.error(err);
      if (onError) onError(err);
    });
}

export { meetUpsApiCall, meetUpApiCall, updateMeetUpApiCall, createMeetUpApiCall }
