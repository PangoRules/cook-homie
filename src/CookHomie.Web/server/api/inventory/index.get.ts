export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()
  const apiUrl = config.apiBaseUrl
  const endpoint = event.path.replace('/api/inventory/', '')
  
  const response = await $fetch(`${apiUrl}/api/inventory${endpoint}`, {
    method: 'GET',
    headers: {
      'Authorization': event.headers.get('authorization') || '',
      'Content-Type': 'application/json'
    }
  })
  
  return response
})