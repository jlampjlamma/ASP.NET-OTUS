export type NotificationVariant = "success" | "error" | "warning";

export type NotificationBoxProps = {
  text: string;
  variant?: NotificationVariant;
};

export const NotificationBox: React.FC<NotificationBoxProps> = ({
  text,
  variant = "success",
}) => {
  // Карта стилей: каждый variant → свои цвета
  const variantStyles = {
    success: { backgroundColor: "#4caf50", color: "white" },
    error: { backgroundColor: "#f44336", color: "white" },
    warning: { backgroundColor: "#ff9800", color: "black" },
  };

  const styles = {
    ...variantStyles[variant],
    padding: "12px",
    borderRadius: "4px",
    marginTop: "16px",
  };

  return <div style={styles}>{text}</div>;
};