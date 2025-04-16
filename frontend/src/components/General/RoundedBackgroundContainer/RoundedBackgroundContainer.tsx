import { Box } from '@mui/material';
import './RoundedBackgroundColor.css'

type RoundedBackgroundContainerProps = {
  height: string;
  backgroundColor: string;
  children: React.ReactNode;
};

const RoundedBackgroundContainer: React.FC<RoundedBackgroundContainerProps> = ({ height, backgroundColor, children }) => {
  return (
    <Box className="container"
      sx={{
        height: height,
        backgroundColor: backgroundColor
      }}
    >
      {children}
    </Box>
  );
};

export default RoundedBackgroundContainer;
