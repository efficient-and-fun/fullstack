import { Box, Typography } from '@mui/material';
import EventCard from "../EventCard/EventCard";
import './DailyView.css';
import { useEffect, useRef, useState } from 'react';

import eventsRaw from './testEvents.json';
import RoundedBackgroundContainer from '../../General/RoundedBackgroundContainer/RoundedBackgroundContainer';
import { Dayjs } from 'dayjs';

interface DailyViewProps {
  selectedDate: Dayjs;
}

const DailyView: React.FC<DailyViewProps> = ({ selectedDate }) => {
  const events = eventsRaw.map(event => ({
    ...event,
    DateTimeFrom: event.DateTimeFrom ? new Date(event.DateTimeFrom) : null,
    DateTimeTo: event.DateTimeTo ? new Date(event.DateTimeTo) : null,
  }));

  const [currentIndex, setCurrentIndex] = useState(0);
  const cardRefs = useRef<(HTMLDivElement | null)[]>([]);
  const scrollRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const observer = new IntersectionObserver(
      (entries) => {
        const visibleEntry = entries.find((entry) => entry.isIntersecting);
        if (visibleEntry) {
          const index = cardRefs.current.findIndex((el) => el === visibleEntry.target);
          if (index !== -1) setCurrentIndex(index);
        }
      },
      {
        root: scrollRef.current,
        threshold: 0.8,
      }
    );

    cardRefs.current.forEach((el) => {
      if (el) observer.observe(el);
    });

    return () => observer.disconnect();
  }, []);



  return (
    <RoundedBackgroundContainer height="45vh" backgroundColor="var(--background-color)">

      <Typography variant="h6" className="daily-view-title">
        {selectedDate.format("DD. MMMM YYYY")}
      </Typography>

      <Box className="daily-view-body">

        <Box className="dot-column">
          {events.map((_, index) => (
            <div key={index} className={`dot ${currentIndex === index ? 'active' : ''}`} />
          ))}
        </Box>

        <Box className="scrollable-cards" ref={scrollRef}>
          {events.map((event, index) => (
            <div
              key={index}
              ref={(el) => {
                cardRefs.current[index] = el;
              }}
            >
              <EventCard meetUp={event} />
            </div>
          ))}
        </Box>
      </Box>
    </RoundedBackgroundContainer>
  );
};

export default DailyView;
