export default {
  api: {
    update: 'api error.',
    create: 'api error.',
    unknown: 'api error.'
  },
  validation: {
    required: 'field is required.',
    maxLength: value => `field must be less than ${value} characters.`
  }
}
