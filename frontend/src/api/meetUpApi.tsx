import { MeetUp } from "../models/MeetUp";
import { MeetUpDetail, NewMeetUpDetail } from "../models/MeetUpDetails";
import { Dayjs } from "dayjs";
import { jwtDecode } from "jwt-decode";

const baseUserURL = "/api/users";

const API_BASE = "/api/meetups";
const AUTH_HEADER = () => ({
  Authorization: `Bearer ${localStorage.getItem("authToken")}`,
});

const parseMeetUpDates = <T extends { dateTimeFrom?: string | Date | null; dateTimeTo?: string | Date | null }>(meetUp: T): T => ({
  ...meetUp,
  dateTimeFrom: meetUp.dateTimeFrom ? new Date(meetUp.dateTimeFrom) : null,
  dateTimeTo: meetUp.dateTimeTo ? new Date(meetUp.dateTimeTo) : null,
});

async function meetUpsApiCall(
  setEvents: React.Dispatch<React.SetStateAction<MeetUp[]>>,
  selectedDate: Dayjs
) {
  const url = `${API_BASE}/?currentDate=${selectedDate.format("YYYY-MM-DD")}`;

  try {
    const res = await fetch(url, {
      method: "GET",
      headers: AUTH_HEADER(),
    });

    if (res.status === 404) return setEvents([]);
    if (!res.ok) throw new Error(`Fehler meetUpsApiCall (Status ${res.status})`);

    const data = (await res.json()) as MeetUp[];
    const meetUps = data.map(parseMeetUpDates);
    setEvents(meetUps);
  } catch (err) {
    console.error("Unexpected error in meetUpsApiCall:", err);
    setEvents([]);
  }
}

async function meetUpApiCall(
  setEvent: React.Dispatch<React.SetStateAction<MeetUpDetail>>,
  id: number
) {
  try {
    const res = await fetch(`${API_BASE}/${id}`, {
      method: "GET",
      headers: AUTH_HEADER(),
    });

    if (!res.ok) throw new Error(`Fehler meetUpApiCall (Status ${res.status})`);

    const data = (await res.json()) as MeetUpDetail;
    setEvent(parseMeetUpDates(data));
  } catch (err) {
    console.error("Unexpected error in meetUpApiCall:", err);
  }
}

async function updateMeetUpApiCall(
  meetUp: MeetUpDetail,
  onSuccess: () => void,
  onError?: (error: unknown) => void
) {
  const url = `${API_BASE}/${meetUp.meetUpId}`;

  try {
    const res = await fetch(url, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        ...AUTH_HEADER(),
      },
      body: JSON.stringify({
        ...meetUp,
        dateTimeFrom: meetUp.dateTimeFrom ? new Date(meetUp.dateTimeFrom).toISOString() : null,
        dateTimeTo: meetUp.dateTimeTo ? new Date(meetUp.dateTimeTo).toISOString() : null,
      }),
    });

    if (!res.ok) throw new Error(`Fehler updateMeetUpApiCall (Status ${res.status})`);

    const contentLength = res.headers.get("Content-Length");
    if (contentLength && parseInt(contentLength) > 0) await res.json();

    onSuccess();
  } catch (err) {
    console.error(err);
    if (onError) onError(err);
  }
}

async function createMeetUpApiCall(
  fullMeetUp: NewMeetUpDetail,
  onSuccess: (newId: number) => void,
  onError?: (error: unknown) => void
) {
  const payload = {
    ...fullMeetUp,
    dateTimeFrom: fullMeetUp.dateTimeFrom ? new Date(fullMeetUp.dateTimeFrom).toISOString() : null,
    dateTimeTo: fullMeetUp.dateTimeTo ? new Date(fullMeetUp.dateTimeTo).toISOString() : null,
    maxNumberOfParticipants: Number(fullMeetUp.maxNumberOfParticipants),
  };

  try {
    const res = await fetch(API_BASE, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        ...AUTH_HEADER(),
      },
      body: JSON.stringify(payload),
    });

    if (!res.ok) throw new Error(`Fehler createMeetUpApiCall (Status ${res.status})`);

    const data = await res.json();
    onSuccess(data);
  } catch (err) {
    console.error("Request failed:", err);
    if (onError) onError(err);
  }
}

async function authApiCall(endpoint: string, body: string) {
  try {
    const url = baseUserURL + endpoint;
    const res = await fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: body,
    });

    const data = await res.json();
    if (res.ok) {
      localStorage.setItem("authToken", data.token);
      saveUserId(data.token);
      return {ok: true, message: "API call succeded."}
    } else {
      return { ok: false, message: data.message };
    }
  } catch (error) {
    // Catch network or other unexpected error:
    console.error("Error during api call:", error);
  }
}

async function saveUserId(tokenBase64: string) {
  interface MyClaims {
    email: string;
    id: string;
    expireDate: string;
  }
  const payload = jwtDecode<MyClaims>(tokenBase64);
  const userId =
    payload[
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
    ];
  localStorage.setItem("userId", userId);
}

export { meetUpsApiCall, meetUpApiCall, createMeetUpApiCall, updateMeetUpApiCall, authApiCall };
