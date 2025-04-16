import { useState } from 'react';
import Calendar from '../components/Calendar/Calendar';
import DailyView from '../components/Home/DailyView/DailyView';
import dayjs, { Dayjs } from 'dayjs';
import HomeHeader from '../components/Home/Header/HomeHeader';

const HomePage = () => {
  const [selectedDate, setSelectedDate] = useState<Dayjs>(dayjs());

  return (
    <>
      <HomeHeader></HomeHeader>
      <Calendar selectedDate={selectedDate} setSelectedDate={setSelectedDate}></Calendar>
      <DailyView selectedDate={selectedDate}></DailyView>
    </>
  );
};

export default HomePage;