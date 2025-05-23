import "./MeetUpDetail.css";
import { useNavigate, useParams } from "react-router-dom";
import { Box, Typography, IconButton, Stack } from "@mui/material";
import AccessTimeIcon from "@mui/icons-material/AccessTime";
import PlaceIcon from "@mui/icons-material/Place";
import CloseIcon from "@mui/icons-material/Close";
import { MeetUpDetail } from "../../models/MeetUpDetails";
import { useState, useEffect } from "react";
import { meetUpApiCall }  from "../../api/meetUpApi";

const MeetUpDetailPage: React.FC = () => {
  const { meetUpId } = useParams<{ meetUpId: string }>();
  const navigate = useNavigate();

  const [event, setEvent] = useState<MeetUpDetail>();

  useEffect(() => {
    const url = "/api/meetUp/1/";
    meetUpApiCall(url, setEvent, parseInt(meetUpId));

  }, [meetUpId]);


  return (
    <Box className="meetup-detail-container">
      {/* Header */}
      <Box className="meetup-header">
        <IconButton
          onClick={() => navigate("/")}
          className="meetup-close-button"
        >
          <CloseIcon />
          <Typography variant="body2">CLOSE</Typography>
        </IconButton>
      </Box>

      {/* Time & Location */}
      <Box className="meetup-info-section">
        {
          <Box className="meetup-info-group">
            <AccessTimeIcon fontSize="small" sx={{ color: "limegreen" }} />
            <Typography variant="body2" className="custom-time-text">
              {event?.dateTimeFrom.toLocaleTimeString([], {
                hour: "2-digit",
                minute: "2-digit",
              })}{" "}
              until{" "}
              {event?.dateTimeTo.toLocaleTimeString([], {
                hour: "2-digit",
                minute: "2-digit",
              })}
            </Typography>
          </Box>
        }
        <Box className="meetup-info-group">
          <PlaceIcon fontSize="small" sx={{ color: "limegreen" }} />
          <Typography color="limegreen">{event?.meetUpLocation}</Typography>
        </Box>
      </Box>

      {/* Title & Description */}
      <Box className="meetup-title-section">
        <Typography variant="h4" fontWeight="bold">
          {event?.meetUpName}
        </Typography>
        {
          <Typography variant="body1" className="meetup-description">
            {event?.description}
          </Typography>
        }
      </Box>
    </Box>
  );
};

export default MeetUpDetailPage;
