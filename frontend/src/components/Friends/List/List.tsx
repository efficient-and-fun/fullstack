import './List.css';
import CustomButton from '../CustomButton/CustomButton';

interface ListProps {
  type: "add" | "delete"; 
  text: string; 
  handleNoItemsText?: string;
  items: string[];
  onButtonClick: (item: string) => void;
}

const List = ({ type, text, handleNoItemsText, items, onButtonClick }: ListProps) => {
  return (
    <div className="list-container">
      <h1>{text}</h1>
      {items.length === 0 ? (
        <span>{handleNoItemsText || 'No items available'}</span>
      ) : (
        <ul>
          {items.map((item, index) => (
            <li key={index} className="list-item">
              {item}
              <CustomButton
                type={type}
                text={type === "add" ? "+" : "-"}
                handleClick={() => onButtonClick(item)}
              />
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default List;