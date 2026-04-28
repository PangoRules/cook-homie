export default defineEventHandler(async (_event) => {
  const config = useRuntimeConfig();

  // This endpoint is only available in dev mode
  if (config.env !== "development") {
    throw createError({
      statusCode: 403,
      statusMessage: "Forbidden",
      message: "Inventory seeding is only available in development mode",
    });
  }

  // Simulate seeding 5 realistic items to the in-memory store
  const items = [
    {
      id: "1",
      name: "Eggs",
      quantity: 12,
      unit: "pieces",
      category: "Dairy",
      expiry: "2024-05-15",
    },
    {
      id: "2",
      name: "Milk",
      quantity: 1,
      unit: "liters",
      category: "Dairy",
      expiry: "2024-05-10",
    },
    {
      id: "3",
      name: "Bread",
      quantity: 1,
      unit: "loaf",
      category: "Bakery",
      expiry: "2024-05-08",
    },
    {
      id: "4",
      name: "Butter",
      quantity: 250,
      unit: "grams",
      category: "Dairy",
      expiry: "2024-05-12",
    },
    {
      id: "5",
      name: "Chicken",
      quantity: 1,
      unit: "kilogram",
      category: "Meat",
      expiry: "2024-05-05",
    },
  ];

  // In a real implementation, this would save to the database
  // For now, we're just returning success message
  return {
    message: "Inventory seeded successfully",
    itemsCount: items.length,
  };
});
