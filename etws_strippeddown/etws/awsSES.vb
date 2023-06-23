'{"notificationType":"Bounce","bounce":{"bounceSubType":"General","bounceType":"Transient","bouncedRecipients":[{"emailAddress":"steve@artechservices.net"}],"timestamp":"2014-05-27T18:31:33.000Z","feedbackId":"000001463ef3a7b1-214715d1-e5cd-11e3-9aad-cb6a8e056c04-000000"},"mail":{"timestamp":"2014-05-27T18:12:29.000Z","source":"alerts@easitrack.com","messageId":"000001463ee22fea-3eedbeb9-5df2-48f3-81d2-17ba0c97d84d-000000","destination":["steve@artechservices.net"]}}

'{
'      "notificationType":"Complaint",
'      "complaint":{
'         "complainedRecipients":[
'            {
'               "emailAddress":"recipient1@example.com"
'            }
'         ],
'         "timestamp":"2012-05-25T14:59:38.613-07:00",
'         "feedbackId":"0000013786031775-fea503bc-7497-49e1-881b-a0379bb037d3-000000"
'      },
'      "mail":{
'         "timestamp":"2012-05-25T14:59:38.613-07:00",
'         "messageId":"0000013786031775-163e3910-53eb-4c8e-a04a-f29debf88a84-000000",
'         "source":"email_1337983178613@amazon.com",
'         "destination":[
'            "recipient1@example.com",
'            "recipient2@example.com",
'            "recipient3@example.com",
'            "recipient4@example.com"
'         ]
'      }
'   }

'{
'"Type" : "SubscriptionConfirmation",
'"MessageId" : "c62d2bec-1223-4634-a29c-a1af17f4ce3f",
'"Token" : "2336412f37fb687f5d51e6e241d638b05b641f3451f55c3fb8fb984796eb632add7eb34a5383494d43f25e18ee3d8ac6b551afb7e41e00fa7fbb4b951a6ea59105284f901796c80aac1574d6e89a31fec760ced44670a66cce070f5628a5650b15d2b1457f3a15458ad100b9703d1b882d8ed24862938722ca8a86eced36b5f7",
'"TopicArn" : "arn:aws:sns:us-east-1:961412317835:EasiTrack_EmailBounces",
'"Message" : "You have chosen to subscribe to the topic arn:aws:sns:us-east-1:961412317835:EasiTrack_EmailBounces.\nTo confirm the subscription, visit the SubscribeURL included in this message.",
'"SubscribeURL" : "https://sns.us-east-1.amazonaws.com/?Action=ConfirmSubscription&TopicArn=arn:aws:sns:us-east-1:961412317835:EasiTrack_EmailBounces&Token=2336412f37fb687f5d51e6e241d638b05b641f3451f55c3fb8fb984796eb632add7eb34a5383494d43f25e18ee3d8ac6b551afb7e41e00fa7fbb4b951a6ea59105284f901796c80aac1574d6e89a31fec760ced44670a66cce070f5628a5650b15d2b1457f3a15458ad100b9703d1b882d8ed24862938722ca8a86eced36b5f7",
'"Timestamp" : "2014-05-27T22:08:06.541Z",
'"SignatureVersion" : "1",
'"Signature" : "mgKLJ1FUcpV4HxIGu+dJjmoADXL2PxzZvM2tIHyIsZATErrXNRmAZ0fsFQky+hB7ad3P7dY8unwdfIFjGnbYhkeiPWvW2zlLKZaPosS5RRZKS6ilTpKxiVKrFmTcof4G/bcPwLAI98XgcDsw5UJ+VqXqK4cqzt2jYasb1nu4emLNMBNQAJRVs4GJgv04GCPzyelNPs5QpV11reT8lGtZlwzorxAKubeW8xo1rFxewWVvarqyN/T/CjftQNNgH63J3HNHvevrULRwpwxLYk0LWz5KQyJ0XdhUH5ZBIfweHplA6M62tSip9rHu9usKJyOHp6kunNCufT/jc7RJncIiqw==",
'"SigningCertURL" : "https://sns.us-east-1.amazonaws.com/SimpleNotificationService-e372f8ca30337fdb084e8ac449342c77.pem"
'}

Imports System.Runtime.Serialization

<DataContract> _
Public Class awsSES

    <DataMember> _
    Public Type As String = ""

    <DataMember> _
    Public SubscribeURL As String = ""

    <DataMember> _
    Public notificationType As String = ""

    <DataMember> _
    Public bounce As awsBounce

    <DataMember> _
    Public complaint As awsComplaint

End Class

<DataContract> _
Public Class awsBounce

    <DataMember> _
    Public bounceSubType As String = ""

    <DataMember> _
    Public bounceType As String = ""

    <DataMember> _
    Public bouncedRecipients As List(Of awsRecipient)

    <DataMember> _
    Public timestamp As String = ""

    <DataMember> _
    Public feedbackId As String = ""

End Class

<DataContract> _
Public Class awsComplaint

    <DataMember> _
    Public complainedRecipients As List(Of awsRecipient)

End Class

<DataContract> _
Public Class awsRecipient

    <DataMember> _
    Public emailAddress As String = ""

End Class