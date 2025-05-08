import { Box, Typography } from "@mui/material";
import EventCard from "../EventCard/EventCard";
import "./DailyView.css";
import { useEffect, useState } from "react";
import { MeetUp } from "../../../models/MeetUp";
import RoundedBackgroundContainer from "../../General/RoundedBackgroundContainer/RoundedBackgroundContainer";
import { Dayjs } from "dayjs";
import { meetUpsApiCall } from "../../../api/meetUpApi";

interface DailyViewProps {
  selectedDate: Dayjs;
}

const DailyView: React.FC<DailyViewProps> = ({ selectedDate }) => {
  const [events, setEvents] = useState<MeetUp[]>([]);

  useEffect(() => {
    const url = "/api/meetUp/1";
    meetUpsApiCall(url, setEvents, selectedDate);

  }, [selectedDate]);

  return (
    <RoundedBackgroundContainer
      height="45vh"
      backgroundColor="var(--background-color)"
    >
      <Typography variant="h6" className="daily-view-title">
        {selectedDate.format("DD. MMMM YYYY")}
      </Typography>

      <Box className="daily-view-body">
        {events.length === 0 ? (
          <Typography className="no-meetups-text">
            No MeetUps on this day
          </Typography>
        ) : (
          <Box className="scrollable-cards" >
            {events.map((event) => (
              <EventCard meetUp={event} />
            ))}
          </Box>
        )}
      </Box>
    </RoundedBackgroundContainer>
  );
};

export default DailyView;
