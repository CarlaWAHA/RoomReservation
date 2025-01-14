export class NotificationService {
  constructor() {
    this.permission = null
  }

  async requestPermission() {
    this.permission = await Notification.requestPermission()
  }

  async sendNotification(title, options = {}) {
    if (this.permission !== 'granted') {
      await this.requestPermission()
    }
    
    return new Notification(title, {
      icon: '/path/to/icon.png',
      ...options
    })
  }
}
