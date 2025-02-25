import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import VCalendar from 'v-calendar'
import 'v-calendar/style.css'
import router from './router'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(VCalendar)
app.use(router)
app.mount('#app')
