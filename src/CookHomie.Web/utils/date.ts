export const formatDate = (dateString: string | null | undefined): string => {
  if (!dateString) {
    return "No expiry date";
  }
  
  const date = new Date(dateString);
  if (isNaN(date.getTime())) {
    return "Invalid date";
  }
  
  return date.toLocaleDateString('en-US', { 
    month: 'short', 
    day: 'numeric', 
    year: 'numeric' 
  });
};