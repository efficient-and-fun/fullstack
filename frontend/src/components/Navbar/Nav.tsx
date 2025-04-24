import * as React from 'react';
import { useNavigate } from 'react-router-dom';
import BottomNavigation from '@mui/material/BottomNavigation';
import BottomNavigationAction from '@mui/material/BottomNavigationAction';
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth';
import GrassIcon from '@mui/icons-material/Grass';
import SettingsIcon from '@mui/icons-material/Settings';
import NotificationsActiveIcon from '@mui/icons-material/NotificationsActive';
import PeopleAltIcon from '@mui/icons-material/PeopleAlt';
import { bottomNavigationActionStyles, bottomNavigationStyles } from './NavStyle';

export default function FixedBottomNavigation() {
  const [value, setValue] = React.useState('events');
  const navigate = useNavigate();

  const handleChange = (_: React.SyntheticEvent, newValue: string) => {
    setValue(newValue);
    navigate(`/${newValue}`);
  };

  return (
    <BottomNavigation sx={bottomNavigationStyles} value={value} onChange={handleChange}>
      
      <BottomNavigationAction
        label="Event"
        value="event"
        icon={<GrassIcon  />}
        sx={bottomNavigationActionStyles}
      />
      <BottomNavigationAction
        label="Home"
        value=""
        icon={<CalendarMonthIcon />}
        sx={bottomNavigationActionStyles}
      />
      <BottomNavigationAction
        label="Notification"
        value="notification"
        icon={<NotificationsActiveIcon />}
        sx={bottomNavigationActionStyles}
      />
      <BottomNavigationAction
        label="Friends"
        value="friends"
        icon={<PeopleAltIcon />}
        sx={bottomNavigationActionStyles}
      />
      <BottomNavigationAction
        label="Setting"
        value="setting"
        icon={<SettingsIcon />}
        sx={bottomNavigationActionStyles}
      />
    </BottomNavigation>
  );
}
