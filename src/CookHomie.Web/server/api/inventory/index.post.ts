export default defineEventHandler(async (event) => {
  const config = useRuntimeConfig()
  const apiUrl = config.apiBaseUrl
  const endpoint = event.path.replace('/api/inventory/', '')
  
  const body = await readBody(event)
  
  const response = await $fetch(`${apiUrl}/api/inventory${endpoint}`, {
    method: 'POST',
    headers: {
      'Authorization': event.headers.get('authorization') || '',
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(body)
  })
  
  return response
})