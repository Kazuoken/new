<script setup>
import { ref, onMounted } from 'vue'

const message = ref('')

onMounted(async () => {
  try {
    const res = await fetch('https://serverapi-fn93.onrender.com')
    message.value = await res.text()
  } catch (err) {
    console.error(err)
  }
})
</script>

<template>
  <div>
    <h1>Vue + ASP.NET CI/CD Demo</h1>
    <p>Backend says: {{ message }}</p>
    <p>Hello every one</p>
  </div>
</template>