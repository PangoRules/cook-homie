export default defineEventHandler(async (_event) => {
  const response = await $fetch("http://api:5000/api/hello", {
    method: "GET",
    headers: {
      Accept: "application/json",
    },
  });

  return response;
});
