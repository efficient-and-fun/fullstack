import './CustomButton.css';

interface CustomButtonProps {
  type: "delete" | "add";
  text: string;
  handleClick: () => void;
}

const CustomButton = ({ type, text, handleClick }: CustomButtonProps) => {
  return (
    <div>
      <button className={`custom-button ${type}`} onClick={handleClick}>
        {text}
      </button>
    </div>
  );
};

export default CustomButton;