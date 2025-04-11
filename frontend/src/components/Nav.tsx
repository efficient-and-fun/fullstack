import * as React from 'react';
import { useNavigate } from 'react-router-dom';
import BottomNavigation from '@mui/material/BottomNavigation';
import BottomNavigationAction from '@mui/material/BottomNavigationAction';
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth';
import GrassIcon from '@mui/icons-material/Grass';
import SettingsIcon from '@mui/icons-material/Settings';
import NotificationsActiveIcon from '@mui/icons-material/NotificationsActive';
import LocationOnIcon from '@mui/icons-material/LocationOn';




export default function FixedBottomNavigation() {
  const [value, setValue] = React.useState('events');
  const navigate = useNavigate();

  const handleChange = (_: React.SyntheticEvent, newValue: string) => {
    setValue(newValue);
    navigate(`/${newValue}`);
  };

  return (
    <BottomNavigation sx={{borderTopLeftRadius: 16, borderTopRightRadius: 16, position: 'fixed',bottom: 0, left:0,right:0}} value={value} onChange={handleChange}>
      
      <BottomNavigationAction
        label="Event"
        value="event"
        icon={<GrassIcon />}
      />
      <BottomNavigationAction
        label="Home"
        value=""
        icon={<CalendarMonthIcon />}
      />
      <BottomNavigationAction
        label="Notification"
        value="notification"
        icon={<NotificationsActiveIcon />}
      />
      <BottomNavigationAction
        label="Friends"
        value="friends"
        icon={<LocationOnIcon />}
      />
      <BottomNavigationAction
        label="Setting"
        value="setting"
        icon={<SettingsIcon />}
      />
    </BottomNavigation>
  );
}
